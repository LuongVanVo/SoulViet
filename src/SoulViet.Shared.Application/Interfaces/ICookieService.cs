namespace SoulViet.Shared.Application.Interfaces;

public interface ICookieService
{
    void SetAuthCookie(string accessToken, string refreshToken);
    void RemoveAuthCookie();
}