# Property-based Tests for the Print Diamond problem

Implementation of the [Print Diamond Kata](https://blog.cyber-dojo.org/2014/10/breaking-down-problem.html)
with [Property-Based Testing](https://arialdomartini.github.io/property-testing).

Ideally, the implementation should be done:

- incrementally activating one test after the other;
- at each step, implementing the bare minumum code to let the test
  pass.
  
This exercise should help figuring out if TDD with Property-Based
Tests can induce an emerging design.

## Build
Run tests:

```bash
dotnet test
```


## Requirements

## Full Diamond Properties

1. Produces a square.
2. Containing more spaces than letters.
3. Contains all the letters from `a` up to the specified `upToLetter` letter.
3. With size `2 * upToLetter - 1`, where `upToLetter` the upToLetter letter number
(`a` is 0).
4. Horizontally specular, with the central element as a pivot (i.e.,
   it does not repeat).
5. Vertically specular, with the cental row as a pivot (i.e., it does
   not repeat).
6. Size is odd (or: each line length is odd).
7. Each line but the first and the last contain a letter repeated twice.


8. All rows have the same length.
9. No letter beyond `upToLetter` is present.
10. No character beyond spaces and letters is present.
11. All letters between `a` and `upToLetter` are present.
12. No row is empty.
13. First and last rows have no inner spaces.
14. Central row has no leading spaces.
15. Central row has no trailing spaces.


Because of `4` and `5`, `4` Quadrants are identified.


## Top-left Quadrant Properties
For the top-left Quadrant: 

16. It is a square.
17. It contains all the letters, up to `upToLetter`.
18. Each line contains a trailing space more than the next one.
19. Each line contains one leading space less than the next one
20. Letters are on the top-right to bottom-left diagonal.
21. Each line contains exactly 1 letter.
22. All the letters from `a` to `upToLetter` are represented.
23. No letter is repeated.

## Diamond

Example:

```
             1111
    1234567890123 
 1 "------a------"
 2 "-----b-b-----"
 3 "----c---c----"
 4 "---d-----d---"
 5 "--e-------e--"
 6 "-f---------f-"
 7 "g-----------g"
 8 "-f---------f-"
 9 "--e-------e--"
10 "---d-----d---"
11 "----c---c----"
12 "-----b-b-----"
13 "------a------"


A quarter:

   1234567
1 "------a"
2 "-----b-"
3 "----c--"
4 "---d---"
5 "--e----"
6 "-f-----"
7 "g------"
```

