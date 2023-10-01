using NUnit.Framework;
using Smtsudoku;
using Program = Smtsudoku.Program;

namespace Tests;

public class Tests
{
    [Test]
    public void TestHard()
    {
        var s = new Solver();
        s.SetSquare(1, 2, 9);
        s.SetSquare(1, 4, 4);
        s.SetSquare(1, 7, 3);
        s.SetSquare(2, 1, 5);
        s.SetSquare(2, 8, 2);
        s.SetSquare(3, 3, 1);
        s.SetSquare(3, 6, 8);
        s.SetSquare(4, 3, 2);
        s.SetSquare(4, 9, 6);
        s.SetSquare(5, 5, 7);
        s.SetSquare(5, 8, 1);
        s.SetSquare(6, 4, 3);
        s.SetSquare(6, 7, 9);
        s.SetSquare(7, 3, 8);
        s.SetSquare(7, 5, 1);
        s.SetSquare(8, 2, 3);
        s.SetSquare(8, 7, 4);
        s.SetSquare(9, 1, 6);
        s.SetSquare(9, 6, 2);
        s.SetSquare(9, 8, 5);
        Assert.IsTrue(s.Solve());
        var soln = s.GetSolution();
        var solnDigits = new string(Program.FormatGrid(soln).Where(char.IsDigit).ToArray());
        Assert.AreEqual("796421385584637129321598674842159736963274518175386942458913267237865491619742853",
            solnDigits);
    }
}