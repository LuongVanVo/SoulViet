namespace SoulViet.Shared.Application.Common.ExternalSettings;

public class CloudflareR2Settings
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string ServiceUrl { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string PublicDomain { get; set; } = string.Empty;
}