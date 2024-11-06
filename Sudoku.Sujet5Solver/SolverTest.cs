namespace Sudoku.Sujet5Solver;

using Python.Runtime;
using Sudoku.Shared;

public class SolverTest: ISudokuSolver
{
 public SudokuGrid Solve(SudokuGrid s)
 {

  return s;
 }
}

public class TestPythonSolver : PythonSolverBase

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

				// Injectez le script de conversion
				// AddNumpyConverterScript(scope);

				// Convertissez le tableau .NET en tableau NumPy
				var pyCells = s.Cells.ToPython();

				// create a Python variable "instance"
				scope.Set("instance", pyCells);

				// run the Python script
				string code = System.IO.File.ReadAllText("SudokuSolver.py");
				scope.Exec(code);

				var result = scope.Get("result").As<int[,]>();

				// Convertissez le résultat NumPy en tableau .NET
				// var managedResult = AsManagedArray(scope, result);

				return new SudokuGrid() { Cells = result };
			}
			//}

		}

		


		protected override void InitializePythonComponents()
		{
			//declare your pip packages here
			// InstallPipModule("numpy");
			base.InitializePythonComponents();
		}

	}

