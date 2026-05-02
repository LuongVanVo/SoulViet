namespace SoulViet.Shared.Application.Features.UserAddressBook.Results;

public class CreateUserAddressResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
}