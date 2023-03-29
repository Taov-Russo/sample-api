using System.Runtime.CompilerServices;

namespace Sample.Api.Infrastructure.Http;

public class Method
{
    public static string GetName([CallerMemberName] string name = null) => name;
}