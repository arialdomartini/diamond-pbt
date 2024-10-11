using System.Collections.Generic;
using System.Linq;

namespace PrintDiamond;

static class PrintDiamond
{
    internal const char Space = '-';
    internal const char Newline = '\n';

    internal static string Print(char upToChar)
    {
        var n = upToChar - 'a' + 1;

        return
            Enumerable.Range(0, n)
                .Select(index =>
                    BuildLine(index, n))
                .Select(SemiDuplicate)
                .SemiDuplicate()
                .Joined();
    }

    private static IEnumerable<char> BuildLine(int index, int n)
    {
        var leadingSpaces = Spaces(n - index - 1);
        var theChar = (char)('a' + index);
        var trailingSpaces = Spaces(index);
        var row = leadingSpaces.Append(theChar).Concat(trailingSpaces);

        return row;
    }

    private static IEnumerable<char> Spaces(int numberOfSpaces) =>
        Enumerable.Repeat(Space, numberOfSpaces);

    private static string Joined(this IEnumerable<IEnumerable<char>> lines) =>
        string.Join(Newline, lines.Select(c => new string(c.ToArray())));

    private static IEnumerable<T> SemiDuplicate<T>(this IEnumerable<T> xs) =>
        xs.Concat(xs.Reverse().Skip(1));
}
