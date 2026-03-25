namespace SoulViet.Modules.Marketplace.Marketplace.Domain.Entities
{
    public class ProductMediaInfo
    {
        public string MainImage { get; set; } = string.Empty;
        public List<string> LandImages { get; set; } = new();
        public string? VideoUrl { get; set; }
    }
}