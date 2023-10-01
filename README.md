# Smtsudoku

This is a sample project that demonstrates basic usage of [Z3](https://github.com/Z3Prover/z3) via its C# API.

See file `Solver.cs` for actual usage of Z3.


## Example execution
```
> cat puzzle
..628....
..91.6...
7........
698...47.
.2..9..8.
.51...239
........5
...5.91..
....647..
> smtsudoku puzzle
Solved puzzle in 0.059 seconds
 1  3  6 │ 2  8  7 │ 9  5  4
 5  8  9 │ 1  4  6 │ 3  2  7
 7  4  2 │ 9  5  3 │ 6  1  8
─────────┼─────────┼─────────
 6  9  8 │ 3  2  5 │ 4  7  1
 3  2  7 │ 4  9  1 │ 5  8  6
 4  5  1 │ 6  7  8 │ 2  3  9
─────────┼─────────┼─────────
 9  6  3 │ 7  1  2 │ 8  4  5
 8  7  4 │ 5  3  9 │ 1  6  2
 2  1  5 │ 8  6  4 │ 7  9  3
>
```

