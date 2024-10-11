module PrintDiamondFSharp.TestHelper

open System
open System.Text.RegularExpressions
open PrintDiamondFSharp.PrintDiamond
open FsCheck

type Row = string
type RowsProperty = Row[] -> bool
type UpToKey = char
type Diamond = string
type Quadrant = string
type DiamondProperty = UpToKey -> Diamond -> bool
type QuadrantProperty = UpToKey -> Quadrant -> bool


let rows (s: Diamond) = s.Split(newLine)

let lettersContainedIn (diamond: Diamond) =
    diamond
    |> Seq.filter (fun c -> c <> space && c <> newLine)
    |> Seq.distinct
    |> Seq.sort
    |> Seq.toArray

let spacesContainedIn (diamond: Diamond) =
    diamond
    |> Seq.filter (fun c -> c = space)

let leadingSpaces (e: Row) =
    e |> Seq.takeWhile ((=) space) |> Seq.length

let trailingSpaces (e: Row) =
    e |> Seq.rev |> Seq.takeWhile ((=) space) |> Seq.length

let isOdd n = n % 2 <> 0

let withoutSpaces (s: Row) =
    s |> Seq.filter (fun c -> c <> space && c <> newLine) |> Seq.toArray

let isPalindrome row =
    if Array.length row % 2 = 0 then false
    else Array.rev row = row

let innerSpaces (input: Row) =
    let m = Regex.Match(input, @"\S(\s+)\S")
    if m.Success then m.Groups[1].Value else String.Empty

let allTheLettersUpTo (upToLetter: char) =
    seq { 'a' .. upToLetter }


let chars = 
    gen {
        let! c = Arb.generate<char>
        return! Gen.elements ['b' .. 'z']
    }

let upToChars = Arb.fromGen chars


let forAllDiamonds (property: DiamondProperty) =
    Prop.forAll upToChars (fun upTo ->
        let diamond = print upTo
        property upTo diamond)

let forAllRows (rowsProperty: RowsProperty) =
    Prop.forAll upToChars (fun upTo ->
        let diamond = print upTo
        let rows = diamond.Split(newLine)
        rowsProperty rows)

let forAllTopLeftQuadrants (property: QuadrantProperty) =
    Prop.forAll upToChars (fun upTo ->
        let diamond = print upTo
        let rows = diamond.Split(newLine)
        let quadrant = 
            rows
            |> Array.take ((Array.length rows / 2) + 1)
            |> Array.map (fun l -> l.[..(String.length l / 2)])
        property upTo (String.Join(newLine, quadrant)))

let forAllRowsInQuadrant (rowsProperty: RowsProperty) =
    Prop.forAll upToChars (fun upTo ->
        let diamond = print upTo
        let rows = diamond.Split(newLine)
        let quadrant = 
            rows
            |> Array.take ((Array.length rows / 2) + 1)
            |> Array.map (fun l -> l.[..(String.length l / 2)])
        rowsProperty quadrant)
