using System;
using System.Diagnostics;
using Sudoku.Shared;
using Python.Runtime;  // Nécessaire pour l'exécution des scripts Python
using System.Collections.Generic;
using System.Text;
using Sudoku.SolverCSPAIMA;

namespace Sudoku.SolverCSPAIMA
{
    public abstract class SolverCSPAIMA : ISudokuSolver
{
    public SolverCSPAIMA()
    {
        _Strategy = GetStrategy();
    }

    private readonly dynamic _Strategy; // Utilisation de 'dynamic' pour interagir avec l'objet Python

    // Résoudre le Sudoku en utilisant la stratégie définie et en appelant le code Python pour la résolution
    public override Shared.SudokuGrid Solve(SudokuGrid s)
    {
        // Appel au script Python pour obtenir le CSP de Sudoku
        return ExecutePythonSolver(s);
    }

    // Méthode abstraite permettant d'obtenir la stratégie spécifique du solver
    protected abstract dynamic GetStrategy(); // Retourne l'instance Python de la stratégie

    // Appel au fichier Python pour résoudre le Sudoku avec la grille
    private Shared.SudokuGrid ExecutePythonSolver(SudokuGrid s)
    {
        using (Py.GIL())
        {
            using (PyModule scope = Py.CreateScope())
            {
                // Injecter les scripts nécessaires, y compris conversionCSP
                AddNumpyConverterScript(scope);
                scope.Exec(Resources.conversionCSP_py);  // Charger conversionCSP.py

                // Convertir la grille de Sudoku en un tableau NumPy
                var pyCells = AsNumpyArray(s.Cells, scope);

                // Créer une variable Python "instance" pour la grille de Sudoku
                scope.Set("instance", pyCells);

                // Charger et exécuter le script Python pour créer le CSP et résoudre le Sudoku
                string code = Resources.infoStratCSP_py;  // Le chemin ou contenu du script Python qui résout le Sudoku
                Console.WriteLine("Exécution du code Python...");
                scope.Exec(code);

                // Récupérer l'instance de la classe Python 'infoStratCSP'
                dynamic strategyInstance = scope.Get("strategyInstance");

                // Appliquer la stratégie via l'instance Python
                dynamic pythonSolver = strategyInstance.GetStrategy();

                // Vérifier si "solved_grid" existe avant de l'extraire
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

                // Retourner la grille de Sudoku avec les valeurs remplies
                return new SudokuGrid() { Cells = managedResult };
            }
        }
    }
}

// Exemple de solver utilisant une combinaison spécifique de stratégies de sélection et d'inférence
public class CSPMRVDegLCVFCSolver : SolverCSPAIMA
{
    protected override dynamic GetStrategy()
    {
        // Créez une instance de infoStratCSP avec les stratégies de sélection et d'inférence
        using (Py.GIL())
        {
            using (PyModule scope = Py.CreateScope())
            {
                // Charger le script Python contenant la classe infoStratCSP
                scope.Exec(Resources.infoStratCSP_py);  // Charger le script Python

                // Créez une instance de la classe Python infoStratCSP
                dynamic objStrategyInfo = scope.Get("infoStratCSP")(); // Créer l'instance de la classe Python infoStratCSP

                // Configurer les propriétés de l'instance Python
                objStrategyInfo.StrategyType = "ImprovedBacktrackingStrategy";
                objStrategyInfo.Selection = "MRVDeg";
                objStrategyInfo.Inference = "ForwardChecking";
                objStrategyInfo.EnableLCV = true;
                objStrategyInfo.MaxSteps = 5000;

                // Retourner l'instance de la stratégie
                return objStrategyInfo;
            }
        }
    }

    // Appel au fichier Python spécifique pour cette stratégie
    protected override SudokuGrid ExecutePythonSolver(SudokuGrid s)
    {
        using (Py.GIL())
        {
            using (PyModule scope = Py.CreateScope())
            {
                // Injecter les scripts nécessaires, y compris SudokuCSPHelper
                AddNumpyConverterScript(scope);
                scope.Exec(Resources.conversionCSP_py);  // Charger SudokuCSPHelper.py

                // Convertir la grille de Sudoku en un tableau NumPy
                var pyCells = AsNumpyArray(s.Cells, scope);

                // Créer une variable Python "instance" pour la grille de Sudoku
                scope.Set("instance", pyCells);

                // Charger et exécuter le script Python pour cette stratégie spécifique
                string code = Resources.infoStratCSP_py;  // Le chemin ou contenu du script Python pour cette stratégie
                Console.WriteLine("Exécution du code Python...");
                scope.Exec(code);

                // Récupérer l'instance de la classe Python 'infoStratCSP'
                dynamic strategyInstance = scope.Get("strategyInstance");

                // Appliquer la stratégie via l'instance Python
                dynamic pythonSolver = strategyInstance.GetStrategy();

                // Vérifier si "solved_grid" existe avant de l'extraire
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

                // Retourner la grille de Sudoku avec les valeurs remplies
                return new SudokuGrid() { Cells = managedResult };
            }
        }
    }
}

    /*

    // Un autre solver avec une stratégie différente pour LCV
    public class CSP2MRVDegFCSolver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = false,              // Ne pas activer LCV
                Inference = CSPInference.ForwardChecking, // Utiliser Forward Checking
                Selection = CSPSelection.MRVDeg, // Sélectionner la variable avec le degré minimal
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy, // Backtracking amélioré
                MaxSteps = 5000                 // Limiter à 5000 étapes
            };
            return objStrategyInfo.GetStrategy();
        }

        // Appel au fichier Python spécifique pour cette stratégie
        protected override SudokuGrid ExecutePythonSolver(SudokuGrid s)
        {
            using (Py.GIL())
            {
                using (PyModule scope = Py.CreateScope())
                {
                    // Injecter les scripts nécessaires, y compris SudokuCSPHelper
                    AddNumpyConverterScript(scope);
                    scope.Exec(Resources.SudokuCSPHelper_py);  // Charger SudokuCSPHelper.py

                    // Convertir la grille de Sudoku en un tableau NumPy
                    var pyCells = AsNumpyArray(s.Cells, scope);

                    // Créer une variable Python "instance" pour la grille de Sudoku
                    scope.Set("instance", pyCells);

                    // Charger et exécuter le script Python pour cette stratégie spécifique
                    string code = Resources.CSP2MRVDegFCSolver_py;  // Le chemin ou contenu du script Python pour cette stratégie
                    Console.WriteLine("Exécution du code Python...");
                    scope.Exec(code);

                    // Vérifier si "solved_grid" existe avant de l'extraire
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

                    // Retourner la grille de Sudoku avec les valeurs remplies
                    return new SudokuGrid() { Cells = managedResult };
                }
            }
        }
    }
    */
}
