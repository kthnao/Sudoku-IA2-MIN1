using Sudoku.Shared;

namespace Sudoku.projettest{

public class MySolver : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
        {
            return s.CloneSudoku();
        }
}
}