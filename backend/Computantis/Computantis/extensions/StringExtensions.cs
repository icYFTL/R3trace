using System.Security.Cryptography;
using System.Text;

namespace Computantis.extensions;

public static class StringExtensions
{
    public static string GetSha512(this string str)
    {
        using var sha = SHA512.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
        
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}