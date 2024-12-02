using Python.Runtime;
using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.SolverCSPAIMA{

public abstract class SolverCSPAIMA : PythonSolverBase
    {
        // Méthode abstraite pour obtenir les stratégies spécifiées
        protected abstract (string heuristic, string valueOrder, string inferenceMethod,  bool useMinConflicts) GetStrategies();

        public override SudokuGrid Solve(SudokuGrid s)
        {
            using (Py.GIL())  // Acquire the GIL (Global Interpreter Lock)
            {
                using (PyModule scope = Py.CreateScope())  // Create a new Python scope
                {
                    // Injecter le script de conversion (conversion entre .NET et NumPy)
                    AddNumpyConverterScript(scope);

                    // Convertir le tableau .NET en tableau NumPy pour l'utiliser dans Python
                    var pyCells = AsNumpyArray(s.Cells, scope);
                    scope.Set("instance", pyCells);

                    // Récupérer les stratégies spécifiées
                    var (selectedHeuristic, selectedValueOrder, selectedInferenceMethod, useMinConflicts) = GetStrategies();

                    // Passer les stratégies sélectionnées au script Python
                    scope.Set("variable_heuristic", selectedHeuristic);
                    scope.Set("value_order", selectedValueOrder);
                    scope.Set("inference_method", selectedInferenceMethod);
                    scope.Set("use_min_conflict", useMinConflicts);

                    // Charger et exécuter le script Python
                    string code = Resources.CSPAIMA_py;  // Le contenu du script Python
                    Console.WriteLine("Exécution du code Python...");
                    scope.Exec(code);

                    // Vérifier si "solved_grid" existe dans le scope
                    if (scope.Contains("solved_grid"))
                    {
                        Console.WriteLine("solved_grid trouvé dans le scope.");
                    }
                    else
                    {
                        Console.WriteLine("solved_grid non trouvé dans le scope.");
                    }

                    // Récupérer le résultat du script Python
                    PyObject result = scope.Get("solved_grid");

                    // Convertir le résultat NumPy en tableau .NET
                    var managedResult = AsManagedArray(scope, result);

                    // Retourner la grille de Sudoku résolue
                    return new SudokuGrid() { Cells = managedResult };
                }
            }
        }

        protected override void InitializePythonComponents()
        {
            // Installer les modules Python nécessaires
            InstallPipModule("numpy");
            InstallPipModule("aima3");
            base.InitializePythonComponents();
        }
    }
}