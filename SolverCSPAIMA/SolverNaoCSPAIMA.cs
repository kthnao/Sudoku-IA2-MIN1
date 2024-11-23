using Sudoku.Shared;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.SolverCSPAIMA
{
    public class SolverNaoCSPAIMA : ISudokuSolver
    {
     
		 public SudokuGrid Solve(SudokuGrid s)
        {
            return s.CloneSudoku();
        }
    }
}
