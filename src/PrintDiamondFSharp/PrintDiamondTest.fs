module PrintDiamondFSharp.PrintDiamondTest

open FsCheck.Xunit

open PrintDiamondFSharp.TestHelper

[<Property>]
let ``produces a square`` () =
    forAllRows (fun rows -> Array.forall (fun line -> String.length line = Array.length rows) rows)

[<Property>]
let ``contains more spaces than letters`` () =
    forAllDiamonds (fun _ diamond ->
        let spaces = spacesContainedIn diamond
        let letters = lettersContainedIn diamond
        (spaces |> Seq.length) >= (letters |> Seq.length))


[<Property>]
let ``contains all the letters from `a` up to the specified `upToLetter` letter`` () =
    forAllDiamonds (fun upTo diamond ->
        let distinctLetters = lettersContainedIn diamond

        let expectedLetters = { 'a' .. upTo } |> Seq.toArray

        distinctLetters = expectedLetters)



[<Property>]
let ``has size 2 * number of letters - 1`` () =
    forAllDiamonds (fun _ diamond ->
        let numberOfDistinctLetters =
            lettersContainedIn diamond |> Seq.distinct |> Seq.length

        diamond
        |> rows
        |> Seq.forall (fun row -> row |> Seq.length = numberOfDistinctLetters * 2 - 1))
