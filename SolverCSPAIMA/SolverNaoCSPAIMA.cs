using Sudoku.Shared;

namespace Sudoku.SolverCSPAIMA{

public class SolverNaoCSPAIMA: ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
        {
            return s.CloneSudoku();
        }

}
}



    

