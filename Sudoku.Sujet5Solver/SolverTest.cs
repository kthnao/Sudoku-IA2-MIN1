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

				// Récupérer l'objet Python
				var result = scope.Get("result");

				var Result = result.As<int[][]>();
				
				//Console.WriteLine($"Type of result from Python: {Result.GetType()}");
				
				// Print the contents of the 2D array
				// foreach (var row in Result)
				// {
				// 	Console.WriteLine(string.Join(", ", row)); // Join the elements of the row as a string
				// }
				
				var managedResult = ConvertJaggedToMultidimensional(Result);

				// Convertissez le résultat NumPy en tableau .NET
				//var managedResult = AsManagedArray(scope, pyCells);

				return new SudokuGrid() { Cells = managedResult };
			}

			// Fonction pour convertir int[][] en int[,]
			static int[,] ConvertJaggedToMultidimensional(int[][] jaggedArray)
			{
				int rows = jaggedArray.Length;
				int columns = jaggedArray[0].Length;

				// Création du tableau multidimensionnel
				int[,] result = new int[rows, columns];

				// Remplissage du tableau multidimensionnel
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < columns; j++)
					{
						result[i, j] = jaggedArray[i][j];
					}
				}

				return result;
			}
			

		}

		


		protected override void InitializePythonComponents()
		{
			//declare your pip packages here
			// InstallPipModule("numpy");
			base.InitializePythonComponents();
		}

	}

