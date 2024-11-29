using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Keras;
using Keras.Models;
using Numpy;
using Python.Runtime;
using Sudoku.Shared;

namespace NeuralNetworkSolverCSharp
{
	public class NeuralNetworkSolverCSharp:ISudokuSolver
	{
		private const string modelPath = @"..\..\..\..\Sudoku.NeuralNetworkSolverCSharp\Models\sudoku.model";
		private static BaseModel model = NeuralNetHelper.LoadModel(modelPath);


		public SudokuGrid Solve(SudokuGrid s)
		{
			return NeuralNetHelper.SolveSudoku(s, model);
		}
	}

	public class NeuralNetHelper
	{

		static NeuralNetHelper()
		{
			PythonEngine.PythonHome = @"C:\ProgramData\Anaconda3";
			Setup.UseTfKeras();
		}

		public static BaseModel LoadModel(string strpath)
		{
			return BaseModel.LoadModel(strpath);
		}

		public static NDarray GetFeatures(SudokuGrid objSudoku)
		{
			return Normalize(np.array(objSudoku.Cells).reshape(9, 9));
		}

		public static NDarray Normalize(NDarray features)
		{
			return (features / 9) - 0.5;
		}

		public static NDarray DeNormalize(NDarray features)
		{
			return (features + 0.5) * 9;
		}



		public static SudokuGrid SolveSudoku(SudokuGrid s, BaseModel model)
		{
			var features = GetFeatures(s);
			while (true)
			{
				var output = model.Predict(features.reshape(1, 9, 9, 1));
				output = output.squeeze();
				var prediction = np.argmax(output, axis: 2).reshape(9, 9) + 1;
				var proba = np.around(np.max(output, axis: new[] { 2 }).reshape(9, 9), 2);

				features = DeNormalize(features);
				var mask = features.@equals(0);
				if (((int)mask.sum()) == 0)
				{
					break;
				}

				var probNew = proba * mask;
				var ind = (int)np.argmax(probNew);
				var (x, y) = ((ind / 9), ind % 9);
				var val = prediction[x][y];
				features[x][y] = val;
				features = Normalize(features);

			}

			return SudokuGrid.ReadSudoku(features.ToString());
		}
	}
}
