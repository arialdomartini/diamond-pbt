using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PrintDiamond;

internal static class TestHelper
{
    internal static string[] Rows(this string s) =>
        s.Split(PrintDiamond.Newline).ToArray();

    internal static char[] LettersContainedIn(string diamond) =>
        diamond
            .Where(c => c != PrintDiamond.Space)
            .Where(c => c != PrintDiamond.Newline)
            .Distinct()
            .Order()
            .ToArray();

    internal static int LeadingSpaces(this string e) =>
        e.TakeWhile(c => c == PrintDiamond.Space).Count();

    internal static int TrailingSpaces(this string e) =>
        e.Reverse().TakeWhile(c => c == PrintDiamond.Space).Count();

    internal static bool IsOdd(this int n) => (n % 2) != 0;

    internal static char[] WithoutSpaces(this string s) => s.Where(c => c != PrintDiamond.Space && c != PrintDiamond.Newline).ToArray();

    internal static bool IsPalindrome<T>(this T[] row)
    {
        if (row.Count() % 2 == 0)
            return false;

        return row.SequenceEqual(row.Reverse());
    }

    internal static string InnerSpaces(this string input)
    {
        var match = Regex.Match(input, @"\S(\s+)\S");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    internal static IEnumerable<char> AllTheLettersUpTo(char upToLetter) =>
        Enumerable.Range('a', upToLetter - 'a' + 1).Select(c => (char)c);
}