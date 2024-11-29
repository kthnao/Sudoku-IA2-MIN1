using Python.Runtime;
using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.SolverCSPAIMA
{
   public class SolverNaoCSPAIMA : PythonSolverBase
    {
        public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
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

                    // --- Définir les heuristiques et stratégies à tester ---
                    // Liste des heuristiques disponibles
                    var heuristics = new List<string> { "mrv", "degree", "mrv_degree" };
                    // Liste des ordres de valeurs possibles
                    var valueOrders = new List<string> { "lcv", "random" };
                    // Liste des stratégies d'inférence possibles
                    var inferenceMethods = new List<string> { "ac3", "mac" };

                    // Sélectionner aléatoirement une combinaison de stratégie
                    Random rand = new Random();
                    string selectedHeuristic = heuristics[rand.Next(heuristics.Count)];
                    string selectedValueOrder = valueOrders[rand.Next(valueOrders.Count)];
                    string selectedInferenceMethod = inferenceMethods[rand.Next(inferenceMethods.Count)];

                    // Passer les stratégies sélectionnées au script Python
                    scope.Set("variable_heuristic", selectedHeuristic);
                    scope.Set("value_order", selectedValueOrder);
                    scope.Set("inference_method", selectedInferenceMethod);

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
