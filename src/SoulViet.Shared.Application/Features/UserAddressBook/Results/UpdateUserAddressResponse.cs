namespace SoulViet.Shared.Application.Features.UserAddressBook.Results;

public class UpdateUserAddressResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
}