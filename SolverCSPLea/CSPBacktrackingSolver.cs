using Sudoku.Shared;

namespace SolverCSPLea;

public class CSPBacktrackingSolver : ISudokuSolver
{



    public SudokuGrid Solve(SudokuGrid s)
    {

        var toSolve = s.CloneSudoku();
        cspSolver(toSolve);
        return toSolve;
    }

    public bool cspSolver(SudokuGrid s)
    {
        Tuple<int, int> emptyCell = FindEmptyCell(s);
        if (emptyCell == null)
            return true;

        int row = emptyCell.Item1;
        int col = emptyCell.Item2;

        for (int i = 1; i <= 9; i++)
        {
            if (IsValidMove(s, row, col, i))
            {
                s.Cells[row][col] = i;

                if (cspSolver(s))
                    return true;

                s.Cells[row][col] = 0;
            }
        }

        return false;
    }

    private Tuple<int, int> FindEmptyCell(SudokuGrid s)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (s.Cells[i][j] == 0)
                    return Tuple.Create(i, j);
            }
        }
        return null;
    }


    private bool IsValidMove(SudokuGrid s, int row, int col, int num)
    {
        return !UsedInRow(s, row, num) && !UsedInCol(s, col, num) &&
               !UsedInSubgrid(s, row, col, num);
    }

    private bool UsedInRow(SudokuGrid s, int row, int num)
    {
        for (int i = 0; i < 9; i++)
            if (s.Cells[row][i] == num)
                return true;
        return false;
    }

    private bool UsedInCol(SudokuGrid s, int col, int num)
    {
        for (int i = 0; i < 9; i++)
            if (s.Cells[i][col] == num)
                return true;
        return false;
    }

    private bool UsedInSubgrid(SudokuGrid s, int row, int col, int num)
    {
        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (s.Cells[i + startRow][j + startCol] == num)
                    return true;
            }
        }
        return false;
    }
}
