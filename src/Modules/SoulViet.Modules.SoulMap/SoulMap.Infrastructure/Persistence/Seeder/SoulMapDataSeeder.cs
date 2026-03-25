using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Enums;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Seeder
{
    public class SoulMapDataSeeder
    {
        private readonly SoulMapDbContext _dbContext;

        public SoulMapDataSeeder(SoulMapDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SeedDataAsync(string touristCsvPath, string accommodationCsvPath)
        {
            // 1. Dọn dẹp dữ liệu cũ (Xóa theo thứ tự để tránh lỗi khóa ngoại)
            await _dbContext.Database.ExecuteSqlRawAsync(@"
                TRUNCATE TABLE soulmap.""TouristAttractions"" CASCADE;
                TRUNCATE TABLE soulmap.""Accommodations"" CASCADE;
                TRUNCATE TABLE soulmap.""Categories"" CASCADE;
                TRUNCATE TABLE soulmap.""Provinces"" CASCADE;
            ");

            // 2. Cache để tránh tạo trùng Provinces & Categories
            var provinceCache = new Dictionary<string, Province>();
            var categoryCache = new Dictionary<string, Category>();

            if (File.Exists(touristCsvPath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    BadDataFound = null,
                    MissingFieldFound = null,
                };

                using var reader = new StreamReader(touristCsvPath);
                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<dynamic>().ToList();
                var attractions = new List<TouristAttraction>();

                foreach (var record in records)
                {
                    var row = (IDictionary<string, object>)record;

                    // Xử lý Province
                    var addressStr = GetString(row, "Address", "Đang cập nhật địa chỉ");
                    var provinceCode = ExtractProvinceCode(addressStr);
                    var province = await EnsureProvinceExistsAsync(provinceCode, provinceCache);

                    // Xử lý Category (Lấy theo Type phân loại, mặc định là Điểm tham quan)
                    var typeStr = GetString(row, "Type", "Điểm tham quan");
                    var category = await EnsureCategoryExistsAsync(typeStr, categoryCache);

                    // Xử lý Tọa độ
                    var lat = TryParseDouble(row, "Lat");
                    var lng = TryParseDouble(row, "Lng");

                    // Xử lý Description (Kết hợp Description và Generated_Description)
                    var originalDesc = GetString(row, "Description");
                    var generatedDesc = GetString(row, "Generated_Description");
                    var combinedDesc = CombineDescriptions(originalDesc, generatedDesc);

                    // Các mảng dữ liệu (JSON)
                    var landImages = ParseStringListFromJson(GetString(row, "LandImages_JSON"));
                    var activities = ParseStringListFromJson(GetString(row, "Activities_JSON"));
                    var reviews = ParseStringListFromJson(GetString(row, "TopReviews_JSON"));
                    var allTypes = ParseStringListFromJson(GetString(row, "AllTypes")) 
                                        ?? new List<string> { typeStr };

                    var attraction = new TouristAttraction
                    {
                        PlaceId = GetString(row, "PlaceId", Guid.NewGuid().ToString()),
                        Name = GetString(row, "Name", "Địa danh chưa xác định"),
                        Address = addressStr,
                        ProvinceId = province.Id,
                        Province = province, // Cho EF theo dõi
                        CategoryId = category.Id,
                        Category = category,
                        Type = typeStr,
                        Description = combinedDesc,
                        OperationHours = GetString(row, "OperationHours", "Đang cập nhật"),
                        Location = new Point(lng, lat) { SRID = 4326 },
                        RatingScore = TryParseDouble(row, "RatingScore"),
                        ReviewCount = TryParseInt(row, "ReviewCount"),
                        ReferencePrice = GetString(row, "PriceRange", GetString(row, "PriceCategory", "Đang cập nhật giá")),
                        AllTypes = allTypes,
                        Activities = activities,
                        TopReviews = reviews,
                        VibeTag = ParseVibeTag(GetString(row, "VibeTag")),
                        BudgetTag = GetString(row, "PriceCategory", "Chưa phân loại"),
                        AiContext = $"Thông tin tổng quan: {combinedDesc}. Hoạt động: {string.Join(", ", activities)}",
                        Media = new PlaceMediaInfo
                        {
                            MainImage = RemoveApiKey(GetString(row, "MainImage", "")),
                            LandImages = landImages.Select(RemoveApiKey).ToList(),
                            VideoUrl = ""
                        }
                    };

                    attractions.Add(attraction);
                }

                if (attractions.Any())
                {
                    await _dbContext.TouristAttractions.AddRangeAsync(attractions);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (File.Exists(accommodationCsvPath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    BadDataFound = null,
                    MissingFieldFound = null,
                };

                using var reader = new StreamReader(accommodationCsvPath);
                using var csv = new CsvReader(reader, config);

                var records = csv.GetRecords<dynamic>().ToList();
                var accommodations = new List<Accommodation>();

                foreach (var record in records)
                {
                    var row = (IDictionary<string, object>)record;

                    // Lấy tỉnh rành mạch từ file
                    var rawProvince = GetString(row, "Tỉnh/Thành phố", GetString(row, "Địa chỉ"));
                    var provinceCode = ExtractProvinceCode(rawProvince);
                    var province = await EnsureProvinceExistsAsync(provinceCode, provinceCache);

                    var lat = TryParseDouble(row, "Tọa độ Lat");
                    var lng = TryParseDouble(row, "Tọa độ Long");

                    var reviewText = GetString(row, "Bình luận (Dùng train AI)", "Chưa có bình luận");
                    var highlights = GetString(row, "Điểm nổi bật", "Chưa có thông tin nổi bật");
                    var combinedAiContext = $"Chỗ ở: {reviewText} | Nổi bật: {highlights}";

                    var accommodation = new Accommodation
                    {
                        Name = GetString(row, "Tên địa điểm", "Chỗ ở chưa xác định"),
                        Address = GetString(row, "Địa chỉ", "Đang cập nhật địa chỉ"),
                        ProvinceId = province.Id,
                        Province = province,
                        Type = ParseAccommodationType(GetString(row, "Loại mô tả (Type)")),
                        PriceValue = TryParseDecimal(row, "Giá trị số"),
                        PriceSegment = GetString(row, "Phân khúc giá", "Chưa xác định"),
                        RatingScore = TryParseDouble(row, "Độ phổ biến (Rating)"),
                        ReviewCount = TryParseInt(row, "Số lượt đánh giá"),
                        StarRating = TryParseInt(row, "Hạng sao") > 0 ? TryParseInt(row, "Hạng sao") : null,
                        ReviewText = reviewText,
                        Highlights = highlights,
                        FacilitiesJson = "[]",
                        BookingUrl = GetString(row, "URL Booking", ""),
                        Location = new Point(lng, lat) { SRID = 4326 },
                        VibeTag = ParseVibeTag(GetString(row, "Tag cảm xúc")),
                        AiContext = combinedAiContext,
                        Media = new PlaceMediaInfo
                        {
                            MainImage = GetString(row, "Link Hình ảnh", ""),
                            LandImages = new List<string>(),
                            VideoUrl = ""
                        }
                    };

                    accommodations.Add(accommodation);
                }

                if (accommodations.Any())
                {
                    await _dbContext.Accommodations.AddRangeAsync(accommodations);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private string CombineDescriptions(string original, string generated)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(original)) parts.Add(original.Trim());
            if (!string.IsNullOrWhiteSpace(generated)) parts.Add(generated.Trim());

            if (!parts.Any()) return "Đang cập nhật mô tả cho địa điểm này.";
            
            return string.Join(" \n\n", parts);
        }

        // Ép dữ liệu về 3 tỉnh duy nhất của dự án
        private string ExtractProvinceCode(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "Đà Nẵng"; // Default
            
            var normalized = text.ToLowerInvariant();
            
            if (normalized.Contains("quảng nam") || normalized.Contains("hội an") || normalized.Contains("tam kỳ"))
                return "Quảng Nam";
                
            if (normalized.Contains("huế") || normalized.Contains("thừa thiên"))
                return "Thừa Thiên Huế";

            // Default fallback
            return "Đà Nẵng";
        }

        private async Task<Province> EnsureProvinceExistsAsync(string rawName, Dictionary<string, Province> cache)
        {
            var slug = rawName.ToLower().Replace(" ", "-").Replace("đ", "d").Replace("ừ", "u").Replace("ộ", "o"); // simplified
            
            if (cache.TryGetValue(slug, out var existing))
                return existing;

            var province = await _dbContext.Provinces.FirstOrDefaultAsync(p => p.Name.Contains(rawName));
            if (province == null)
            {
                province = new Province
                {
                    Id = Guid.NewGuid(),
                    Name = rawName,
                    Slug = slug
                };
                await _dbContext.Provinces.AddAsync(province);
                await _dbContext.SaveChangesAsync();
            }

            cache[slug] = province;
            return province;
        }

        private async Task<Category> EnsureCategoryExistsAsync(string categoryName, Dictionary<string, Category> cache)
        {
            var slug = categoryName.ToLower().Replace(" ", "-");
            if (cache.TryGetValue(slug, out var existing))
                return existing;

            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = categoryName,
                    Slug = slug,
                    IconUrl = ""
                };
                await _dbContext.Categories.AddAsync(category);
                await _dbContext.SaveChangesAsync();
            }

            cache[slug] = category;
            return category;
        }

        private List<string> ParseStringListFromJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
            }
            catch
            {
                // Thử fallback cắt bằng dấu phẩy nếu không phải JSON
                return jsonString.Split(',')
                                 .Select(x => x.Trim().Trim('"').Trim('\''))
                                 .Where(x => !string.IsNullOrEmpty(x))
                                 .ToList();
            }
        }

        private AccommodationType ParseAccommodationType(string typeStr)
        {
            if (string.IsNullOrWhiteSpace(typeStr)) return AccommodationType.KhachSan;
            
            var t = typeStr.ToLowerInvariant();
            if (t.Contains("homestay")) return AccommodationType.Homestay;
            if (t.Contains("resort") || t.Contains("khu nghỉ dưỡng")) return AccommodationType.Resort;
            if (t.Contains("villa") || t.Contains("biệt thự")) return AccommodationType.Villa;
            
            return AccommodationType.KhachSan;
        }

        private VibeTag ParseVibeTag(string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue)) return VibeTag.TraiNghiemDaDang; // Hoặc kiểu enum default của bạn

            var lower = strValue.ToLower();
            if (lower.Contains("chữa lành") || lower.Contains("yên bình")) return VibeTag.ChuaLanhVaYenBinh;
            if (lower.Contains("năng động") || lower.Contains("phiêu lưu")) return VibeTag.NangDongVaPhieuLuu;
            if (lower.Contains("sang trọng") || lower.Contains("đẳng cấp")) return VibeTag.SangTrongVaDangCap;
            if (lower.Contains("sáng tạo") || lower.Contains("truyền cảm hứng")) return VibeTag.SangTaoVaTruyenCamHung;
            if (lower.Contains("văn hóa") || lower.Contains("bản địa")) return VibeTag.DamVanHoaVaBanDia;

            return VibeTag.TraiNghiemDaDang; // Fallback an toàn thay vì bắn exception
        }

        private string GetString(IDictionary<string, object> dict, string key, string fallback = "")
        {
            if (dict.TryGetValue(key, out var value) && value != null)
            {
                var str = value.ToString()?.Trim();
                if (!string.IsNullOrEmpty(str)) return str;
            }
            return fallback;
        }

        private double TryParseDouble(IDictionary<string, object> dict, string key)
        {
            var strValue = GetString(dict, key);
            if (double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
            return 0;
        }

        private int TryParseInt(IDictionary<string, object> dict, string key)
        {
            var strValue = GetString(dict, key);
            if (double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return (int)result;
            return 0;
        }

        private decimal TryParseDecimal(IDictionary<string, object> dict, string key)
        {
            var strValue = GetString(dict, key);
            if (decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;
            return 0;
        }

        private string RemoveApiKey(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;
            return url.Split("&key=")[0];
        }
    }
}