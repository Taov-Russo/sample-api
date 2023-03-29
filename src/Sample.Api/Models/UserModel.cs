using System;
using Newtonsoft.Json;

namespace Sample.Api.Models;

public class UserModel
{
    public Guid UserId { get; set; }
    public string Login { get; set; }
    [JsonIgnore]
    public byte[] PasswordHash { get; set; }
}