using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Services;

public class MediaMigrationService
{
    private readonly SoulMapDbContext _dbContext;
    public MediaMigrationService(SoulMapDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task MigrateMediaUrlAsync(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            throw new FileNotFoundException($"Media migration file not found: {jsonFilePath}");
        }

        var jsonString = await File.ReadAllTextAsync(jsonFilePath);
        var migrationData = JsonSerializer.Deserialize<List<MediaMigrationDto>>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (migrationData == null || !migrationData.Any())
        {
            Console.WriteLine("Media migration skipped: no records found in JSON.");
            return;
        }

        var updatedCount = 0;
        var notFoundCount = 0;
        var invalidIdentifierCount = 0;

        foreach (var item in migrationData)
        {
            TouristAttraction? attraction = null;
            var hasLookupValue = false;

            if (Guid.TryParse(item.Id, out var attractionId))
            {
                hasLookupValue = true;
                attraction = await _dbContext.TouristAttractions
                    .FirstOrDefaultAsync(x => x.Id == attractionId);
            }

            if (attraction == null && Guid.TryParse(item.PlaceId, out var placeGuid))
            {
                hasLookupValue = true;
                attraction = await _dbContext.TouristAttractions
                    .FirstOrDefaultAsync(x => x.Id == placeGuid);
            }

            if (attraction == null && !string.IsNullOrWhiteSpace(item.PlaceId))
            {
                hasLookupValue = true;
                attraction = await _dbContext.TouristAttractions
                    .FirstOrDefaultAsync(x => x.PlaceId == item.PlaceId);
            }

            if (!hasLookupValue)
            {
                invalidIdentifierCount++;
                continue;
            }

            if (attraction != null)
            {
                var currentMedia = attraction.Media ?? new PlaceMediaInfo();

                currentMedia.MainImage = item.MainImage?.Trim() ?? string.Empty;
                currentMedia.LandImages = (item.LandImages ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();

                attraction.Media = currentMedia;
                updatedCount++;
            }
            else
            {
                notFoundCount++;
            }
        }

        var affectedRows = await _dbContext.SaveChangesAsync();
        Console.WriteLine(
            $"Media migration completed. Total={migrationData.Count}, Updated={updatedCount}, NotFound={notFoundCount}, InvalidIdentifier={invalidIdentifierCount}, DbChanges={affectedRows}.");
    }
}
