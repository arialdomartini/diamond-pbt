using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using static System.Linq.Enumerable;
using static PrintDiamond.PrintDiamond;
using static PrintDiamond.TestHelper;

namespace PrintDiamond;

public class PrintDiamondKataTest
{
    delegate bool RowsProperty(string[] strings);

    delegate bool DiamondProperty(char upTo, string diamond);

    delegate bool QuadrantProperty(char upTo, string diamond);

    private static Gen<char> Chars =>
        from c in Arb.Generate<char>()
        where c >= 'b'
        where c <= 'z'
        select c;

    private static Arbitrary<char> UpToChars => Chars.ToArbitrary();

    private static Property ForAllDiamonds(DiamondProperty property) =>
        Prop.ForAll(UpToChars, upTo =>
        {
            var diamond = Print(upTo);

            return property(upTo, diamond);
        });

    private static Property ForAllRows(RowsProperty rowsProperty) =>
        Prop.ForAll(UpToChars, upTo =>
        {
            var diamond = Print(upTo);

            var rows = diamond.Rows();

            return rowsProperty(rows);
        });

    private static string[] Quadrant(string diamond)
    {
        var rows = diamond.Rows();
        var quadrant =
            rows
                .Take(rows.Length / 2 + 1)
                .Select(l => l[..(l.Length / 2 + 1)]).ToArray();

        return quadrant;
    }

    private static Property ForAllTopLeftQuadrants(QuadrantProperty property) =>
        Prop.ForAll(UpToChars, upTo =>
        {
            var diamond = Print(upTo);

            var quadrant = Quadrant(diamond);

            return property(upTo, string.Join(Newline, quadrant));
        });

    private static Property ForAllRowsInQuadrant(RowsProperty rowsProperty) =>
        Prop.ForAll(UpToChars, upTo =>
        {
            var diamond = Print(upTo);

            var quadrant = Quadrant(diamond);

            return rowsProperty(quadrant);
        });

    // 1. Produces a square.
    [Property]
    Property is_a_square() =>
        ForAllRows(rows =>
            rows.All(line => line.Length == rows.Length));

    // 2. Containing more spaces than letters.
    [Property]
    Property more_spaces_than_letters() =>
        ForAllDiamonds((_, diamond) =>
        {
            var spaces = diamond.Count(c => c == Space);
            var letters = LettersContainedIn(diamond);
            return spaces >= letters.Length;
        });

    // 3. Contains all the letters from `a` up to the specified `upToLetter` letter.
    [Property]
    Property contains_all_the_letters_up_to_upTo() =>
        ForAllDiamonds((upTo, diamond) =>
        {
            var distinctLetters = LettersContainedIn(diamond);


            IEnumerable<char> AllLetters(char from, char upTo)
            {
                for (var c = from; c <= upTo; c++)
                    yield return c;
            }

            var expectedLetters = AllLetters('a', upTo).ToArray();

            return
                distinctLetters.SequenceEqual(expectedLetters);
        });

    // 3. With size `2 * number of letters - 1
    [Property]
    Property has_size_2_times_minus_1_the_number_of_letters() =>
        ForAllDiamonds((_, diamond) =>
        {
            var numberOfDistinctLetters = LettersContainedIn(diamond).Distinct().Count();

            return
                diamond.Rows()
                    .All(row => row.Length == numberOfDistinctLetters * 2 - 1);
        });


    // 4. Horizontally specular, with the central element as a pivot (i.e., it does not repeat).
    [Property]
    Property semi_symmetric_horizontally() =>
        ForAllRows(rows =>
            rows.All(row => row.ToArray().IsPalindrome()));

    // 5. Vertically specular, with the central row as a pivot (i.e., it does not repeat).
    [Property]
    Property semi_symmetric_vertically() =>
        ForAllRows(rows =>
            rows.IsPalindrome());

    // 6. Size is odd (or: each line length is odd).
    [Property]
    Property size_is_odd() =>
        ForAllRows(rows =>
            rows.Length.IsOdd() && rows.All(line => line.Length.IsOdd()));

    // 7. Each line but the first and the last contain a letter repeated twice.
    [Property]
    Property each_line_but_the_first_and_the_last_contain_a_letter_repeated_twice() =>
        ForAllRows(rows =>
            rows.Skip(1).SkipLast(1).All(
                line =>
                {
                    var withoutSpaces = line.WithoutSpaces();
                    return withoutSpaces[0] == withoutSpaces[1];
                })
        );

    // 8. All rows have the same length.
    [Property]
    Property all_rows_have_the_same_length() =>
        ForAllRows(rows =>
            rows
                .Select(s => s.Length)
                .Distinct()
                .Count() == 1);

    // 9. No letter beyond `upToLetter` is present.
    [Property]
    Property no_letter_beyond_upToLetter_is_present() =>
        ForAllDiamonds((upToLetter, diamond) =>
            LettersContainedIn(diamond).All(c => c <= upToLetter));

    // 10. No character beyond spaces and letters is present.
    [Property]
    Property no_character_beyond_spaces_and_letters_is_present() =>
        ForAllRows(rows =>
            rows.All(row => LettersContainedIn(row).All(c => char.IsLetter(c) || c == Space)));

    // 11. All letters between `a` and `upToLetter` are present.
    [Property]
    Property all_letters_between_a_and_upToLetter_are_present() =>
        ForAllDiamonds((upToLetter, diamond) =>
            LettersContainedIn(diamond)
                .SequenceEqual(
                    AllTheLettersUpTo(upToLetter)));

    // 12. No row is empty.
    [Property]
    Property no_row_is_empty() =>
        ForAllRows(rows =>
            rows.All(row => row.Length > 0));

    // 13. First and last rows have no inner spaces.
    [Property]
    Property first_and_last_rows_have_no_inner_spaces() =>
        ForAllRows(rows =>
        {
            string[] firstAndLast = [rows.First(), rows.Last()];

            return firstAndLast.All(s => s.InnerSpaces().Length == 0);
        });


    // 14. Central row has no leading spaces.
    [Property]
    Property central_row_has_no_leading_spaces() =>
        ForAllRows(rows =>
        {
            var centralRow = rows[rows.Length / 2];

            return centralRow.LeadingSpaces() == 0;
        });


    // Because of `4` and `5`, `4` Quadrants are identified.

    // ## Top-left Quadrant Properties
    // For the top-left Quadrant: 
    //
    // 16. It is a square.
    [Property]
    Property quadrant_is_a_square() =>
        ForAllRowsInQuadrant(rows =>
            rows.All(line => line.Length == rows.Length));

    // 17. It contains all the letters, up to `upToLetter`.
    [Property]
    Property top_left_quadrant_contains_all_letters_between_a_and_upToLetter() =>
        ForAllTopLeftQuadrants((upToLetter, quadrant) =>
        {
            var allTheLetters = Range('a', upToLetter - 'a' + 1).Select(c => (char)c);

            int[] numbers = { 2, 3, 4, 5 };

            return LettersContainedIn(quadrant)
                .SequenceEqual(allTheLetters);
        });

    // 18. Each line contains a trailing space more than the next one.
    [Property]
    Property in_top_left_quadrant_each_line_contains_1_leading_space_more_than_the_next_one() =>
        ForAllRowsInQuadrant(rows =>
        {
            var firstHalf = rows;
            var shifted = rows.Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.All(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.LeadingSpaces() == next.LeadingSpaces() + 1;
            });
        });

    // 19. Each line contains one leading space less than the next one
    [Property]
    Property in_top_left_quadrant_each_line_contains_1_trailing_space_less_than_the_next_one() =>
        ForAllRowsInQuadrant(rows =>
        {
            var firstHalf = rows;
            var shifted = rows.Skip(1);
            var together = firstHalf.Zip(shifted);

            return together.All(el =>
            {
                var previous = el.Item1;
                var next = el.Item2;

                return previous.TrailingSpaces() + 1 == next.TrailingSpaces();
            });
        });

    // 20. Letters are on the top-right to bottom-left diagonal.
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

        string DiagonalOf(string[] rows)
        {
            var n = rows.Length;

            return new string(Range(0, n)
                .Select(i => rows[i][n - 1 - i])
                .ToArray());
        }

        return ForAllRowsInQuadrant(rows =>
        {
            var n = rows.Length;

            var diagonal = DiagonalOf(rows);

            IEnumerable<char> enumerable = diagonal.Distinct();
            var containSpaceInDiagonal = enumerable.Contains(Space);

            return !containSpaceInDiagonal && OnlySpacesBesidesTheDiagonal(rows);
        });
    }

    // 21. Each line contains exactly 1 letter.
    [Property]
    Property in_top_left_quadrant_each_line_contains_exactly_1_letter() =>
        ForAllRowsInQuadrant(rows =>
            rows.All(row => LettersContainedIn(row).Length == 1));

    // 22. All the letters from `a` to `upToLetter` are represented.
    [Property]
    Property in_the_top_left_quadrant_all_letters_between_a_and_upToLetter_are_present() =>
        ForAllTopLeftQuadrants((upToLetter, quadrant) =>
            LettersContainedIn(quadrant)
                .SequenceEqual(
                    AllTheLettersUpTo(upToLetter)));

    // 23. No letter is repeated.
    [Property]
    Property in_the_top_left_quadrant_no_letter_is_repeated() =>
        ForAllTopLeftQuadrants((_, quadrant) =>
        {
            var letters = LettersContainedIn(quadrant);

            return letters.All(letter => quadrant.Count(c => c == letter) == 1);
        });
}
