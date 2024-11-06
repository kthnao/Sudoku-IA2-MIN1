using Python.Runtime;
using Sudoku.Shared;

namespace Sudoku.ResolutionNeuronne;

public class SolveurNeuronne : ISudokuSolver 
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        return s;
    }

}


public class SolveurNeuronnePython : PythonSolverBase
{

	public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
	{
		//System.Diagnostics.Debugger.Break();

		//For some reason, the Benchmark runner won't manage to get the mutex whereas individual execution doesn't cause issues
		//using (Py.GIL())
		//{
		// create a Python scope
		using (PyModule scope = Py.CreateScope())
		{
			// convert the Person object to a PyObject
			PyObject pySudoku = s.Cells.ToPython();

			// create a Python variable "person"
			scope.Set("sudoku", pySudoku);

			// the person object may now be used in Python
            String code = System.IO.File.ReadAllText("SolveurNeuronnePython.py");
			scope.Exec(code);
            var result = scope.Get("result").As<int[,]>();
			return new SudokuGrid() { Cells = result };
		}
		//}

	}

}