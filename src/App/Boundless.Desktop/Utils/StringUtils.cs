namespace Boundless.Desktop.Utils;

public static class StringUtils
{
    public static string CapitalizeWord(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }

    public static string RandomHexString(int length)
    {
        var buffer = new byte[length];
        new Random().NextBytes(buffer);

        return Convert.ToHexString(buffer);
    }
}