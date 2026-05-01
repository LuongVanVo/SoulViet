namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class ProductAttributeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = "[]";
    public int SortOrder { get; set; } = 0;
}