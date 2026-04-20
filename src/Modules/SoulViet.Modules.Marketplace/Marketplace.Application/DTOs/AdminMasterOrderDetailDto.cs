namespace SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;

public class AdminMasterOrderDetailDto : MasterOrderDetailDto
{
    public Guid UserId { get; set; }
    public string? AdminNotes { get; set; }
}