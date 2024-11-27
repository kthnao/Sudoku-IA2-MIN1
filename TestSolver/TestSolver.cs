using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Shared;

namespace TestSolver
{
	public class TestSolver : ISudokuSolver
	{
		/// <summary>
		/// Solves the given Sudoku grid using human techniques.
		/// </summary>
		/// <param name="s">The Sudoku grid to be solved.</param>
		/// <returns>
		/// The solved Sudoku grid.
		/// </returns>
		public SudokuGrid Solve(SudokuGrid s)
		{
			bool progress;
			do
			{
				progress = ApplyTechniques(s);

				// If no techniques work but the puzzle isn't solved, it’s unsolvable with logic
				if (!progress && !IsSolved(s))
				{
					throw new InvalidOperationException("This puzzle requires guessing or advanced techniques not supported by this solver.");
				}

			} while (!IsSolved(s));

			return s;
		}

		/// <summary>
		/// Applies human solving techniques to the grid.
		/// </summary>
		/// <param name="s">The Sudoku grid.</param>
		/// <returns>True if progress was made, false otherwise.</returns>
		private bool ApplyTechniques(SudokuGrid s)
		{
			// Try each technique in order of simplicity
			if (ApplyNakedSingles(s)) return true;
			if (ApplyHiddenSingles(s)) return true;
			// Add more techniques here as needed (e.g., Naked Pairs, Hidden Pairs, etc.)

			return false; // No progress made
		}

		/// <summary>
		/// Checks if the grid is solved.
		/// </summary>
		private bool IsSolved(SudokuGrid s)
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (s.Cells[row, col] == 0) return false; // Unsigned cell means it's unsolved
				}
			}
			return true;
		}

		/// <summary>
		/// Identifies and solves cells using the Naked Singles technique.
		/// </summary>
		/// <param name="s">The Sudoku grid.</param>
		/// <returns>True if any cell was solved, false otherwise.</returns>
		private bool ApplyNakedSingles(SudokuGrid s)
		{
			bool progress = false;

			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (s.Cells[row, col] == 0) // Only consider empty cells
					{
						var candidates = GetCandidates(s, row, col);
						if (candidates.Count == 1)
						{
							// Naked single found
							s.Cells[row, col] = candidates.First();
							progress = true;
						}
					}
				}
			}

			return progress;
		}

		/// <summary>
		/// Identifies and solves cells using the Hidden Singles technique.
		/// </summary>
		/// <param name="s">The Sudoku grid.</param>
		/// <returns>True if any cell was solved, false otherwise.</returns>
		private bool ApplyHiddenSingles(SudokuGrid s)
		{
			bool progress = false;

			for (int region = 0; region < 27; region++) // 9 rows + 9 columns + 9 blocks
			{
				var emptyCells = GetCellsInRegion(s, region)
					.Where(cell => s.Cells[cell.row, cell.col] == 0)
					.ToList();

				var candidatesMap = new Dictionary<int, List<(int row, int col)>>();

				// Map candidates to their positions in the region
				foreach (var (row, col) in emptyCells)
				{
					foreach (var candidate in GetCandidates(s, row, col))
					{
						if (!candidatesMap.ContainsKey(candidate))
						{
							candidatesMap[candidate] = new List<(int, int)>();
						}
						candidatesMap[candidate].Add((row, col));
					}
				}

				// Look for Hidden Singles (candidates appearing exactly once in the region)
				foreach (var kvp in candidatesMap)
				{
					if (kvp.Value.Count == 1)
					{
						var (row, col) = kvp.Value.First();
						s.Cells[row, col] = kvp.Key;
						progress = true;
					}
				}
			}

			return progress;
		}

		/// <summary>
		/// Gets the list of candidates for a specific cell.
		/// </summary>
		private List<int> GetCandidates(SudokuGrid s, int row, int col)
		{
			var candidates = new List<int>();

			for (int val = 1; val <= 9; val++)
			{
				if (IsValid(s, row, col, val))
				{
					candidates.Add(val);
				}
			}

			return candidates;
		}

		/// <summary>
		/// Checks if a value is valid in the given cell.
		/// </summary>
		private bool IsValid(SudokuGrid s, int row, int col, int val)
		{
			// Check row
			for (int c = 0; c < 9; c++)
				if (s.Cells[row, c] == val) return false;

			// Check column
			for (int r = 0; r < 9; r++)
				if (s.Cells[r, col] == val) return false;

			// Check 3x3 block
			int blockRow = (row / 3) * 3;
			int blockCol = (col / 3) * 3;
			for (int r = 0; r < 3; r++)
			{
				for (int c = 0; c < 3; c++)
				{
					if (s.Cells[blockRow + r, blockCol + c] == val) return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets all cells in a specific region (row, column, or block).
		/// </summary>
		private IEnumerable<(int row, int col)> GetCellsInRegion(SudokuGrid s, int region)
		{
			if (region < 9) // Row
			{
				for (int col = 0; col < 9; col++)
				{
					yield return (region, col);
				}
			}
			else if (region < 18) // Column
			{
				int col = region - 9;
				for (int row = 0; row < 9; row++)
				{
					yield return (row, col);
				}
			}
			else // Block
			{
				int block = region - 18;
				int blockRow = (block / 3) * 3;
				int blockCol = (block % 3) * 3;
				for (int r = 0; r < 3; r++)
				{
					for (int c = 0; c < 3; c++)
					{
						yield return (blockRow + r, blockCol + c);
					}
				}
			}
		}
	}
}

