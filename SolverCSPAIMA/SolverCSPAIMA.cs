using Sudoku.Shared;

namespace Sudoku.SolverCSPAIMA{

public class SolverCSPAIMA : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
        {
            return s.CloneSudoku();
        }
}
}