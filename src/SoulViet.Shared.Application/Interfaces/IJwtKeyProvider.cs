using System.Security.Cryptography;

namespace SoulViet.Shared.Application.Interfaces;

public interface IJwtKeyProvider
{
    RSA GetPrivateKey();
    RSA GetPublicKey();
}