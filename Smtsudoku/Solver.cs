using Microsoft.Z3;

namespace Smtsudoku;

public class Solver : IDisposable
{
    internal Context Z3Context { get; }
    internal Microsoft.Z3.Solver Z3Solver { get; }
    internal IntExpr[,] Squares { get; }
    private int[,] Solution;

    public const int ROWS = 9;
    public const int COLUMNS = 9;


    public Solver()
    {
        Z3Context = new Context(new Dictionary<string, string>() { { "model", "true" } });
        Z3Solver = Z3Context.MkSolver();

        // Create a Z3 value for each square.
        Squares = new IntExpr[ROWS, COLUMNS];
        for (int row = 1; row <= ROWS; row++)
        for (int col = 1; col <= COLUMNS; col++)
            Squares[row - 1, col - 1] = Z3Context.MkIntConst($"{row}_{col}");
        Solution = new int[ROWS, COLUMNS];
        AddSudokuRules();
    }

    private void AddSudokuRules()
    {
        var s = Z3Solver;
        var ctx = Z3Context;
        var one = ctx.MkInt(1);
        var nine = ctx.MkInt(9);

        for (int rowIdx = 0; rowIdx < ROWS; rowIdx++)
        for (int colIdx = 0; colIdx < COLUMNS; colIdx++)
        {
            var value = Squares[rowIdx, colIdx];

            // Constrain the value to be an integer in [1, 9].
            s.Assert(ctx.MkAnd(ctx.MkGe(value, one), ctx.MkLe(value, nine)));
        }

        // Assert squares in a row are distinct.
        for (int rowIdx = 0; rowIdx < ROWS; ++rowIdx)
        {
            var rowSquares = new Expr[COLUMNS];
            for (int colIdx = 0; colIdx < COLUMNS; ++colIdx)
            {
                rowSquares[colIdx] = Squares[rowIdx, colIdx];
            }

            s.Assert(ctx.MkDistinct(rowSquares));
        }

        // Assert squares in a column are distinct.
        for (int colIdx = 0; colIdx < COLUMNS; ++colIdx)
        {
            var colSquares = new Expr[ROWS];
            for (int rowIdx = 0; rowIdx < ROWS; ++rowIdx)
            {
                colSquares[rowIdx] = Squares[rowIdx, colIdx];
            }

            s.Assert(ctx.MkDistinct(colSquares));
        }

        // Assert squares in the nine boxes are distinct.
        for (int rowIdx = 0; rowIdx < ROWS; rowIdx += 3)
        for (int colIdx = 0; colIdx < COLUMNS; colIdx += 3)
            AssertBoxDistinct(rowIdx, colIdx);

        void AssertBoxDistinct(int topLeftRowIdx, int topLeftColIdx)
        {
            var boxSquares = new List<Expr>(9);
            for (int rowOffset = 0; rowOffset < 3; ++rowOffset)
            for (int colOffset = 0; colOffset < 3; ++colOffset)
                boxSquares.Add(Squares[topLeftRowIdx + rowOffset, topLeftColIdx + colOffset]);
            s.Assert(ctx.MkDistinct(boxSquares));
        }
    }

    public void SetSquare(int row, int column, int value)
    {
        var square = Squares[row - 1, column - 1];
        Z3Solver.Assert(Z3Context.MkEq(square, Z3Context.MkInt(value)));
    }

    // Returns true on success.
    public bool Solve()
    {
        var ok = Z3Solver.Check();
        if (ok != Status.SATISFIABLE)
            return false;
        var stats = Z3Solver.Statistics;
        var model = Z3Solver.Model;
        for (int rowIdx = 0; rowIdx < ROWS; ++rowIdx)
        for (int colIdx = 0; colIdx < COLUMNS; ++colIdx)
            Solution[rowIdx, colIdx] = ((IntNum)model.Eval(Squares[rowIdx, colIdx])).Int;

        return true;
    }

    public int[,] GetSolution() => (int[,])Solution.Clone();
    

    public void Dispose()
    {
        Z3Solver.Dispose();
        Z3Context.Dispose();
    }
}