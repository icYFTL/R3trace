using System.Security.Cryptography;

namespace R3TraceShared.utils;

public static class GeneralUtils
{
    public static string RandomString(int bytesCount)
    {
        using var rngCryptoServiceProvider = RandomNumberGenerator.Create();
        var randomBytes = new byte[bytesCount];
        rngCryptoServiceProvider.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }
    
    public static double TextSimilarity(string str1, string str2)
    {
        int len1 = str1.Length;
        int len2 = str2.Length;
        int[,] matrix = new int[len1 + 1, len2 + 1];

        for (int i = 0; i <= len1; i++)
        {
            matrix[i, 0] = i;
        }

        for (int j = 0; j <= len2; j++)
        {
            matrix[0, j] = j;
        }

        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                int cost = (str2[j - 1] == str1[i - 1]) ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost
                );
            }
        }

        double maxLen = Math.Max(len1, len2);
        double similarity = (maxLen - matrix[len1, len2]) / maxLen;

        return similarity;
    }


}