namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class CartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public List<CartItemDto> Items { get; set; } = new();

    public decimal GrandTotal => Items.Sum(x => x.SubTotal);
}