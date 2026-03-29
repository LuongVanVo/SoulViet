using System.Security.Cryptography;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Authentication;

public class RsaKeyProvider : IJwtKeyProvider
{
    private readonly string _privateKeyPath = "keys/private_key.pem";
    private readonly string _publicKeyPath = "keys/public_key.pem";
    private RSA? _rsa;
    public RsaKeyProvider()
    {
        if (!Directory.Exists("keys")) Directory.CreateDirectory("keys");

        if (!File.Exists(_privateKeyPath) || !File.Exists(_publicKeyPath))
        {
            GenerateAndSaveKeys();
        }
    }

    public RSA GetPrivateKey()
    {
        if (_rsa != null) return _rsa;
        LoadKeys();
        return _rsa!;
    }

    public RSA GetPublicKey()
    {
        if (_rsa == null) LoadKeys();
        var publicKey = RSA.Create();
        publicKey.ImportFromPem(File.ReadAllText(_publicKeyPath));
        return publicKey;
    }

    private void GenerateAndSaveKeys()
    {
        using var rsa = RSA.Create(2048);
        File.WriteAllText(_privateKeyPath, rsa.ExportRSAPrivateKeyPem());
        File.WriteAllText(_publicKeyPath, rsa.ExportRSAPublicKeyPem());
    }

    private void LoadKeys()
    {
        _rsa = RSA.Create();
        _rsa.ImportFromPem(File.ReadAllText(_privateKeyPath));
    }
}