using System;

namespace Sample.Api.Models.Configuration;

public class JwtConfiguration
{
    public TimeSpan TokenLifetime { get; set; }
    public string EncryptionKey { get; set; }
    public string ValidAudience { get; set; }
    public string ValidIssuer { get; set; }
}