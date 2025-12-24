using System.Security.Cryptography;
using System.Text;

namespace Boundless.Backend.Utils;

public static class StringUtils
{
    public static string ComputeHash(this string str)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));
        var builder = new StringBuilder();
        
        foreach (var b in bytes)
            builder.Append(b.ToString("x2"));
        
        return builder.ToString();
    }
    
    public static string RandomHex(int len)
    {
        var rand = new Random();
        var bytes = new byte[len];
        rand.NextBytes(bytes);

        return Convert.ToHexStringLower(bytes);
    }
    
    public static byte[] RandomHexBytes(int len)
    {
        var rand = new Random();
        var bytes = new byte[len];
        rand.NextBytes(bytes);

        return bytes;
    }
    
    public static string RandomHexAsBase64(int len)
    {
        var bytes = RandomHexBytes(len);
        return Convert.ToBase64String(bytes);
    }
}