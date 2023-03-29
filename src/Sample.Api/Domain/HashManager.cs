using System.Security.Cryptography;
using System.Text;

namespace Sample.Api.Domain;

public class HashManager
{
    public static byte[] GetPasswordHash512(string password)
    {
        using var sha512 = new SHA512CryptoServiceProvider();
        return sha512.ComputeHash(Encoding.Unicode.GetBytes(password));
    }
}