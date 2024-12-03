using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Shared;

namespace Sudoku.ProjectTestSolver
{
    public class NorvigNumericSolver : ISudokuSolver
    {
        private const int Size = 9;
        private const int TotalCells = Size * Size;

        private static readonly int[][] Units = CreateUnits();
        private static readonly HashSet<int>[] Peers = CreatePeers();

        public SudokuGrid Solve(SudokuGrid s)
        {
            var grid = ParseGrid(s);
            if (grid == null)
            {
                Console.WriteLine("Invalid grid provided.");
                return s;
            }

            var result = Search(grid);
            if (result != null)
            {
                ApplySolution(s, result);
            }
            else
            {
                Console.WriteLine("No solution found.");
            }

            return s;
        }

        private int[][] ParseGrid(SudokuGrid s)
        {
            var possibleValues = Enumerable.Range(0, TotalCells)
                                           .Select(_ => Enumerable.Range(1, Size).ToArray())
                                           .ToArray();

            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    int value = s.Cells[r, c];
                    if (value != 0)
                    {
                        if (!Assign(possibleValues, Index(r, c), value))
                        {
                            return null;
                        }
                    }
                }
            }

            return possibleValues;
        }

        private int[][] Search(int[][] grid)
        {
            if (grid.All(cell => cell.Length == 1))
            {
                return grid;
            }

            int box = Array.FindIndex(grid, cell => cell.Length > 1);
            foreach (var value in grid[box])
            {
                var copy = grid.Select(cell => cell.ToArray()).ToArray();
                if (Assign(copy, box, value))
                {
                    var result = Search(copy);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private bool Assign(int[][] grid, int box, int value)
        {
            foreach (var other in grid[box].Where(v => v != value).ToArray())
            {
                if (!Eliminate(grid, box, other))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Eliminate(int[][] grid, int box, int value)
        {
            if (!grid[box].Contains(value))
            {
                return true;
            }

            grid[box] = grid[box].Where(v => v != value).ToArray();
            if (grid[box].Length == 0)
            {
                return false;
            }

            if (grid[box].Length == 1)
            {
                var soleValue = grid[box][0];
                foreach (var peer in Peers[box])
                {
                    if (!Eliminate(grid, peer, soleValue))
                    {
                        return false;
                    }
                }
            }

            foreach (var unit in Units.Where(u => u.Contains(box)))
            {
                var places = unit.Where(b => grid[b].Contains(value)).ToArray();
                if (places.Length == 0)
                {
                    return false;
                }

                if (places.Length == 1)
                {
                    if (!Assign(grid, places[0], value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ApplySolution(SudokuGrid s, int[][] solution)
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    s.Cells[r, c] = solution[Index(r, c)][0];
                }
            }
        }

        private static int Index(int row, int col) => row * Size + col;

        private static int[][] CreateUnits()
        {
            var units = new List<int[]>();

            for (int i = 0; i < Size; i++)
            {
                units.Add(Enumerable.Range(i * Size, Size).ToArray());
                units.Add(Enumerable.Range(i, TotalCells).Where(j => j % Size == i).ToArray());
            }

            for (int br = 0; br < Size; br += 3)
            {
                for (int bc = 0; bc < Size; bc += 3)
                {
                    units.Add(Enumerable.Range(0, TotalCells)
                                        .Where(i => i / Size / 3 == br / 3 && i % Size / 3 == bc / 3)
                                        .ToArray());
                }
            }

            return units.ToArray();
        }

        private static HashSet<int>[] CreatePeers()
        {
            var peers = new HashSet<int>[TotalCells];
            for (int i = 0; i < TotalCells; i++)
            {
                peers[i] = new HashSet<int>(Units.Where(u => u.Contains(i))
                                                 .SelectMany(u => u)
                                                 .Where(j => j != i));
            }

            return peers;
        }
    }
}
