using Sudoku.Shared;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.SolverCSPAIMA
{
    public class SolverNaoCSPAIMA : PythonSolverBase

    {
      public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
        {
            using (PyModule scope = Py.CreateScope())
            {
                // Convertir la grille Sudoku de .NET en tableau NumPy pour Python
                AddNumpyConverterScript(scope);
                var pyCells = AsNumpyArray(s.Cells, scope);

                // Passer la grille au script Python
                scope.Set("instance", pyCells);

                // Code Python utilisant AIMA pour résoudre le Sudoku via CSP
                string code = Resources.CSPAIMA_py;
                scope.Exec(code);

                // Récupérer le résultat du Sudoku résolu
                PyObject result = scope.Get("result");

                // Convertir la grille NumPy en tableau .NET
                var managedResult = AsManagedArray(scope, result);
                return new SudokuGrid() { Cells = managedResult };
            }
        }

        protected override void InitializePythonComponents()
        {
            // Installer les modules Python nécessaires (comme numpy et aima3)
            InstallPipModule("numpy");
            InstallPipModule("aima3");
            base.InitializePythonComponents();
        }

	}
}
    
