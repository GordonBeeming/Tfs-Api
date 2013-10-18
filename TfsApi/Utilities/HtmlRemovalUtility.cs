using System;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Methods to remove HTML from strings.
/// </summary>
internal static class HtmlRemovalUtility
{
    /// <summary>
    /// Remove HTML from string with Regex.
    /// </summary>
    public static string StripTagsRegex(string source)
    {
        return Regex.Replace(source, "<.*?>", string.Empty);
    }

    /// <summary>
    /// Compiled regular expression for performance.
    /// </summary>
    static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

    /// <summary>
    /// Remove HTML from string with compiled Regex.
    /// </summary>
    public static string StripTagsRegexCompiled(string source)
    {
        return _htmlRegex.Replace(source, string.Empty);
    }

    /// <summary>
    /// Remove HTML tags from string using char array.
    /// </summary>
    public static string StripTagsCharArray(string source, bool replaceNewLineWithEmpty = true)
    {
        char[] array = new char[source.Length];
        int arrayIndex = 0;
        bool inside = false;

        for (int i = 0; i < source.Length; i++)
        {
            char let = source[i];
            if (let == '<')
            {
                inside = true;
                continue;
            }
            if (let == '>')
            {
                inside = false;
                continue;
            }
            if (!inside)
            {
                array[arrayIndex] = let;
                arrayIndex++;
            }
        }
        return new string(array, 0, arrayIndex).Replace("\n",string.Empty);
    }

    /// <summary>
    /// Remove HTML tags from string using char array with special parsing at the end.
    /// </summary>
    public static string StripTagsCharArrayWithExtraParsing(string source)
    {
        string newSource = string.Empty;
        foreach (char ch in HttpUtility.HtmlDecode(StripTagsCharArray(source)))
        {
            if (char.IsLetter(ch) || char.IsDigit(ch))
            {
                newSource += ch;
            }
        }
        return newSource;
    }
}