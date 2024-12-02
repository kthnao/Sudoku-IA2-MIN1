using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Python.Runtime;
using Sudoku.Shared;
using System.Text;


namespace Sudoku.simulé
{
    public class SimuléPythonSolver : PythonSolverBase
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
				AddNumpyConverterScript(scope);

				// Convertissez le tableau .NET en tableau NumPy
				var pyCells = AsNumpyArray(s.Cells, scope);

				// create a Python variable "instance"
				scope.Set("instance", pyCells);

				// run the Python script
				string code = Resources.Simulé_py;
               
				scope.Exec(code);

				PyObject result = scope.Get("result");

				// Convertissez le résultat NumPy en tableau .NET
				var managedResult = AsManagedArray(scope, result);

				return new SudokuGrid() { Cells = managedResult };
			}
			//}

		}

		


		protected override void InitializePythonComponents()
		{
			//declare your pip packages here
			InstallPipModule("numpy");
			base.InitializePythonComponents();
		}

	}
}

        
