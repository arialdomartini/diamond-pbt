module PrintDiamondFSharp.PrintDiamond

let space = '-'
let newLine = '\n'

let spaces numberOfSpaces =
    String.replicate numberOfSpaces (string space)

let buildLine index n =
    let leadingSpaces = spaces (n - index - 1)
    let theChar = char (int 'a' + index)
    let trailingSpaces = spaces index
    leadingSpaces + string theChar + trailingSpaces

let semiDuplicate xs =
    Seq.append xs (Seq.rev xs |> Seq.skip 1)

let print (upToChar: char) =
    let length = int upToChar - int 'a' + 1

    let semiDuplicate =
        [ 0 .. length - 1 ]
        |> Seq.map (fun index -> buildLine index length)
        |> Seq.map semiDuplicate
        |> Seq.map System.String.Concat
        |> semiDuplicate

    semiDuplicate |> fun s -> System.String.Join(newLine, s)
