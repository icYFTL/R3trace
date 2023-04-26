namespace Computantis.utils;

public static class Utils
{
    public static string GenerateRandomString(int length)
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = allowedChars[random.Next(allowedChars.Length)];
        }

        return new string(result);
    }
}