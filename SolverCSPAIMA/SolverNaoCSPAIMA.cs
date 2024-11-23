using Sudoku.Shared;
using Python.Runtime;

namespace Sudoku.SolverCSPAIMA
{
    public class SolverNaoCSPAIMA : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            using (Py.GIL())
            {
                dynamic pySolver = Py.Import("aima_csp_solver");
                var pyResult = pySolver.solve_sudoku(s.Cells);

                int[,] result = ConvertPyObjectTo2DArray(pyResult);
                return new SudokuGrid() { Cells = result };
            }
        }

        private int[,] ConvertPyObjectTo2DArray(dynamic pyResult)
        {
            int[,] result = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result[i, j] = (int)pyResult[i][j];
                }
            }
            return result;
        }
    }
}
