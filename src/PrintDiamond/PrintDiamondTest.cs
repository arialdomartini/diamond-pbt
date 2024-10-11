using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using static System.Linq.Enumerable;
using static PrintDiamond.PrintDiamond;

namespace PrintDiamond;

internal static class TestHelper
{
    internal static string[] Lines(this string s) =>
        s.Split(Newline).ToArray();

    internal static char[] ContainedLetters(this string diamond) =>
        diamond
            .Where(c => c != Space)
            .Where(c => c != Newline)
            .Distinct()
            .Order()
            .ToArray();

    internal static string Reversed(this string line) =>
        new(line.Reverse().ToArray());


    internal static int LeadingSpaces(this string e) =>
        e.TakeWhile(c => c == Space).Count();

    internal static int TrailingSpaces(this string e) =>
        e.Reverse().TakeWhile(c => c == Space).Count();
}

public class PrintDiamondKataTest
{
    delegate bool LinesProperty(string[] strings);

    delegate bool DiamondProperty(char target, string diamond);

    private static Gen<char> Chars =>
        from c in Arb.Generate<char>()
        where c >= 'b'
        where c <= 'z'
        select c;

    private static Arbitrary<char> TargetChars => Chars.ToArbitrary();

    private static Property ForEachDiamond(DiamondProperty property) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = Print(target);

            return property(target, diamond);
        });

    private static Property ForAllLines(LinesProperty linesProperty) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = Print(target);

            var lines = diamond.Lines();

            return linesProperty(lines);
        });

    private static Property ForAllLinesInQuadrant(LinesProperty linesProperty) =>
        Prop.ForAll(TargetChars, target =>
        {
            var diamond = Print(target);

            var lines = diamond.Lines();
            var quadrant =
                lines
                    .Take(lines.Length / 2 + 1)
                    .Select(l => l[..(l.Length / 2 + 1)]).ToArray();

            return linesProperty(quadrant);
        });

    [Property]
    Property is_a_square() =>
        ForAllLines(lines =>
            lines.All(line => line.Length == lines.Length));

    [Property]
    Property more_spaces_than_letters() =>
        ForEachDiamond((_, diamond) =>
        {
            var spaces = diamond.Count(c => c == Space);
            var letters = diamond.ContainedLetters();
            return spaces >= letters.Length;
        });

    [Property]
    Property contains_all_the_letters_up_to_target() =>
        ForEachDiamond((target, diamond) =>
        {
            var distinctLetters = diamond.ContainedLetters();


            IEnumerable<char> AllLetters(char from, char upTo)
            {
                for (var c = from; c <= upTo; c++)
                    yield return c;
            }

            var expectedLetters = AllLetters('a', target).Order().ToArray();

            return
                distinctLetters.SequenceEqual(expectedLetters);
        });

    [Property]
    Property semi_symmetric_horizontally() =>
        ForAllLines(lines =>
            lines.All(IsPalyndrome));

    private bool IsPalyndrome(string line) =>
        line == line.Reversed();

    [Property]
    Property semi_symmetric_vertically() =>
        ForAllLines(lines =>
            lines.SequenceEqual(lines.Reverse()));

    [Property]
    Property in_top_left_quadrant_each_line_contains_1_leading_space_more_than_the_next_one() =>
        ForAllLinesInQuadrant(lines =>
        {
            var firstHalf = lines;
            var shifted = lines.Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.All(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.LeadingSpaces() == next.LeadingSpaces() + 1;
            });
        });

    [Property]
    Property in_top_left_quadrant_each_line_contains_1_trailing_space_less_than_the_next_one() =>
        ForAllLinesInQuadrant(lines =>
        {
            var firstHalf = lines;
            var shifted = lines.Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.All(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.TrailingSpaces() + 1 == next.TrailingSpaces();
            });
        });

    [Property]
    Property in_top_left_quadrant_letters_are_on_the_top_right_to_bottom_left_diagonal()
    {
        bool OnlySpacesBesidesTheDiagonal(string[] strings)
        {
            var size = strings.Length;

            var elementsBesidesDiagonal =
                from row in Range(0, size)
                from column in Range(0, size)
                where row != size - column - 1
                select strings.ElementAt(row)[column];

            return elementsBesidesDiagonal.All(ch => ch == Space);
        }

        string DiagonalOf(string[] lines)
        {
            var n = lines.Length;
            
            return new string(Range(0, n)
                .Select(i => lines[i][n - 1 - i])
                .ToArray());
        }

        return ForAllLinesInQuadrant(lines =>
        {
            var n = lines.Length;

            var diagonal = DiagonalOf(lines);

            IEnumerable<char> enumerable = diagonal.Distinct();
            var containSpaceInDiagonal = enumerable.Contains(Space);

            return !containSpaceInDiagonal && OnlySpacesBesidesTheDiagonal(lines);
        });
    }
}
