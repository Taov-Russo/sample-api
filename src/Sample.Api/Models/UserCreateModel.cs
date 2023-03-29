using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Models;

public class UserCreateModel : JsonModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}