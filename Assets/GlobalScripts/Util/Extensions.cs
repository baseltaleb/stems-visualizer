using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string GetFileName(this string filePath)
    {
        string pattern = @"(?:[/\\]|^)([^/\\]+)$";
        Match match = Regex.Match(filePath, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty; // Return empty string if no match found
    }
}