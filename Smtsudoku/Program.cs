// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;

namespace Smtsudoku;

public class Program
{
    public static void ShowUsage()
    {
        Console.WriteLine($"Usage: {nameof(Smtsudoku)} [file path]\n");
        Console.WriteLine("The file should have 9 lines (rows), each 9 columns in length.");
        Console.WriteLine("Each digit (or missing digit) should be one character in size.");
    }

    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            ShowUsage();
            return 1;
        }

        var filePath = args[0];
        int[,] grid;
        try
        {
            grid = ParseGrid(filePath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reading puzzle from {filePath}: {ex.Message}");
            return 1;
        }
         
        using var solver = new Solver();
        for (int rowIdx = 0; rowIdx < 9; ++rowIdx)
        for (int colIdx = 0; colIdx < 9; ++colIdx)
        {
            var value = grid[rowIdx, colIdx];
            if (value >= 1 && value <= 9)
                solver.SetSquare(rowIdx + 1, colIdx + 1, value);
        }

        var sw = new Stopwatch();
        sw.Start();
        if (!solver.Solve())
        {
            Console.Error.WriteLine("Failed to solve puzzle");
            return 1;
        }

        sw.Stop();
        Console.WriteLine($"Solved puzzle in {sw.Elapsed.TotalSeconds:f3} seconds");
        Console.WriteLine(FormatGridWithLines(solver.GetSolution()));
        return 0;
    }

    public static int[,] ParseGrid(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var ret = new int[9, 9];
        bool begun = false;
        int rowIdx = 0;
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine().Trim();
            if (!begun && line.Length == 0)
                continue;
            if (line.Length < 9)
                throw new ParseException($"Found fewer than 9 characters in row {rowIdx + 1}");
            for (int colIdx = 0; colIdx < 9; ++colIdx)
            {
                var c = line[colIdx];
                if (char.IsDigit(c))
                    ret[rowIdx, colIdx] = c - '0';
            }

            ++rowIdx;
        }

        return ret;
    }

    public static string FormatGrid(int[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var ret = new StringBuilder();
        for (int rowIdx = 0; rowIdx < rows; ++rowIdx)
        {
            for (int colIdx = 0; colIdx < cols; ++colIdx)
                ret.Append($" {grid[rowIdx, colIdx]} ");
            ret.AppendLine();
        }

        return ret.ToString();
    }

    public static string FormatGridWithLines(int[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        var ret = new StringBuilder();
        for (int rowIdx = 0; rowIdx < rows; ++rowIdx)
        {
            for (int colIdx = 0; colIdx < cols; ++colIdx)
            {
                ret.Append($" {grid[rowIdx, colIdx]} ");
                if ((colIdx + 1) % 3 == 0 && colIdx > 0 && colIdx < 8)
                    ret.Append('\u2502');
            }

            ret.AppendLine();

            if ((rowIdx + 1) % 3 == 0 && rowIdx > 0 && rowIdx < 8)
                ret.AppendLine("\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u253c" +
                               "\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u253c" +
                               "\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500");
        }

        return ret.ToString();
    }
}