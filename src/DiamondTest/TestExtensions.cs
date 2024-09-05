using System.Collections.Generic;
using System.Linq;

namespace DiamondTest;

static class TestExtensions
{
    private static string Stringified(this IEnumerable<char> cs) =>
        string.Join("", cs);

    internal static IEnumerable<char> DifferentLetters(this IList<string> rows) =>
        rows.IgnoringSpaces().SelectMany(row => row.Distinct()).Distinct();

    internal static IEnumerable<string> FirstHalf(this IList<string> list) =>
        list.Take(list.Count / 2);

    internal static IEnumerable<string> SecondHalf(this IList<string> list) =>
        list.Skip(list.Count / 2 + 1);

    internal static string Center(this IList<string> list) =>
        list.Skip(list.Count / 2).First();

    internal static int LeadingSpaces(this string e) =>
        e.TakeWhile(c => c == DiamondPrint.Space).Count();

    internal static int TrailingSpaces(this string e) =>
        e.Reverse().TakeWhile(c => c == DiamondPrint.Space).Count();

    internal static int InnerSpaces(this string e)
    {
        var withoutLeadingSpaces = e
            .SkipWhile(c => c == DiamondPrint.Space)
            .Stringified();
        var withoutTrailingSpaces =
            withoutLeadingSpaces
                .Reverse()
                .SkipWhile(c => c == DiamondPrint.Space)
                .Stringified();
        var withoutNonSpaces = withoutTrailingSpaces
            .Where(c => c == DiamondPrint.Space)
            .Stringified();
        
        return withoutNonSpaces.Length;
    }

    private static string WithoutSpaces(this string s) =>
        string.Join("", s.Where(c => c != DiamondPrint.Space));

    internal static IList<string> IgnoringSpaces(this IEnumerable<string> s) =>
        s.Select(WithoutSpaces).ToList();

    internal static bool IsOdd(this int i) => i % 2 == 1;

    internal static IEnumerable<string> Inside(this IEnumerable<string> list) =>
        list.Skip(1).SkipLast();
}
