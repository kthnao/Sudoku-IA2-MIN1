using Python.Runtime;
using Sudoku.Shared;
using System;
using System.Collections.Generic;

namespace Sudoku.SolverCSPAIMA
{
    public class SudokuSolverBase : PythonSolverBase
    {
        // Ces propriétés définissent les stratégies d'heuristique, d'ordre de valeurs et d'inférence
        protected string VariableHeuristic { get; set; }
        protected string ValueOrder { get; set; }
        protected string InferenceMethod { get; set; }

        // Constructeur qui reçoit les stratégies à utiliser
        public SudokuSolverBase(string variableHeuristic, string valueOrder, string inferenceMethod)
        {
            VariableHeuristic = variableHeuristic;
            ValueOrder = valueOrder;
            InferenceMethod = inferenceMethod;
        }

        // Méthode principale pour résoudre la grille
        public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
        {
            using (Py.GIL())
            {
                using (PyModule scope = Py.CreateScope())
                {
                    // Injecter le script de conversion
                    AddNumpyConverterScript(scope);
                    var pyCells = AsNumpyArray(s.Cells, scope);
                    scope.Set("instance", pyCells);

                    // Passer les stratégies sélectionnées au script Python
                    scope.Set("variable_heuristic", VariableHeuristic);
                    scope.Set("value_order", ValueOrder);
                    scope.Set("inference_method", InferenceMethod);

                    // Charger et exécuter le script Python
                    string code = Resources.CSPAIMA_py;
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

                    // Récupérer et convertir le résultat du script Python
                    PyObject result = scope.Get("solved_grid");
                    var managedResult = AsManagedArray(scope, result);

                    // Retourner la grille de Sudoku résolue
                    return new SudokuGrid() { Cells = managedResult };
                }
            }
        }

        // Initialisation des composants Python nécessaires
        protected override void InitializePythonComponents()
        {
            InstallPipModule("numpy");
            InstallPipModule("aima3");
            base.InitializePythonComponents();
        }
    }
}
