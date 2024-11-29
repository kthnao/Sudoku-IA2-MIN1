using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Shared;

namespace TestSolver
{
	public class TestSolver : ISudokuSolver
	{

		public SudokuGrid Solve(SudokuGrid s)
		{
			return s.CloneSudoku();
		}
	}

}
