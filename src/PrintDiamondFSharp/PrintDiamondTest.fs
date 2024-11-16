module PrintDiamondFSharp.PrintDiamondTest

open FsCheck.Xunit

open PrintDiamondFSharp.TestHelper

[<Property>]
let ``1. Produces a square`` () =
    everySetOfRows (fun rows ->
            rows |> Array.forall (fun line -> String.length line = Array.length rows))


[<Property>]
let ``2. Contains more spaces than letters`` () =
    everyDiamond (fun _ diamond ->
        let spaces = spacesContainedIn diamond
        let letters = lettersContainedIn diamond
        (spaces |> Seq.length) >= (letters |> Seq.length))


[<Property>]
let ``3. Contains all the letters from `a` up to the specified `upToLetter` letter`` () =
    everyDiamond (fun upTo diamond ->
        let distinctLetters = lettersContainedIn diamond

        let expectedLetters = { 'a' .. upTo } |> Seq.toArray

        distinctLetters = expectedLetters)


[<Property>]
let ``4. Has size 2 * number of letters - 1`` () =
    everyDiamond (fun _ diamond ->
        let numberOfDistinctLetters =
            lettersContainedIn diamond |> Seq.distinct |> Seq.length

        diamond
        |> rows
        |> Seq.forall (fun row -> row |> Seq.length = numberOfDistinctLetters * 2 - 1))


[<Property>]
let ``5. Horizontally specular, with the central element as a pivot (i.e., it is not repeated)`` () =
    everySetOfRows (fun rows ->
        rows |> Seq.forall isPalindrome)


[<Property>]
let ``6. Vertically specular, with the central row as a pivot (i.e., it is not repeated)`` () =
    everySetOfRows isPalindrome


[<Property>]
let ``7. Size is odd (or: each line length is odd)`` () =
    everySetOfRows (fun rows ->
        rows |> Seq.length |> isOdd &&
        rows |> Seq.forall (Seq.length >> isOdd));

[<Property>]
let ``8. Each line but the first and the last contain a letter repeated twice`` () =
    everySetOfRows (fun rows ->
        rows
        |> Seq.skipFirst
        |> Seq.skipLast
        |> Seq.forall (fun line ->
            let withoutSpaces = line |> withoutSpaces;
            withoutSpaces[0] = withoutSpaces[1]))
