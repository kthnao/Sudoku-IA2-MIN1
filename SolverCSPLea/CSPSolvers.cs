using System;
using System.Diagnostics;
using aima.core.search.csp;
using Sudoku.Shared;


namespace Sudoku.CSPSolver
{
    public abstract class CSPSolverBase : ISudokuSolver
    {

        public CSPSolverBase()
        {
            _Strategy = GetStrategy();
        }

        private readonly SolutionStrategy _Strategy;

        // Vous n'avez plus qu'à suivre les commentaires, ça devrait aller vite.
        public SudokuGrid Solve(SudokuGrid s)
        {
            //Construction du CSP en utilisant CspHelper
             CSP csp = new CSP();
             csp = SudokuCSPHelper.GetSudokuCSP(s);

            // Utilisation de la stratégie pour résoudre le CSP
            Assignment ass = _Strategy.solve(csp);

            //Utilisation de CSPHelper pour traduire l'assignation en SudokuGrid
            SudokuCSPHelper.SetValuesFromAssignment(ass, s);

            return s;
        }

        protected abstract SolutionStrategy GetStrategy();

    }

    //Voilà un premier solver, je vous laisse implémenter les autres avec différentes combinaisons de paramètres pour pouvoir contraster leurs performances
    public class CSPMRVDegLCVFCSolver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = true,
                Inference = CSPInference.ForwardChecking,
                Selection = CSPSelection.MRVDeg,
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy,
                MaxSteps = 5000
            };
            return objStrategyInfo.GetStrategy();
        }
    }

    public class CSP2MRVDegFCSolver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = false,
                Inference = CSPInference.ForwardChecking,
                Selection = CSPSelection.MRVDeg,
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy,
                MaxSteps = 5000
            };
            return objStrategyInfo.GetStrategy();
        }
    }

    public class CSP3MRVDegLCVAC3Solver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = true,
                Inference = CSPInference.AC3,
                Selection = CSPSelection.MRVDeg,
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy,
                MaxSteps = 5000
            };
            return objStrategyInfo.GetStrategy();
        }
    }

    public class CSP4MRVLCVFCSolver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = true,
                Inference = CSPInference.ForwardChecking,
                Selection = CSPSelection.MRV,
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy,
                MaxSteps = 5000
            };
            return objStrategyInfo.GetStrategy();
        }
    }

    //Celui-ci avait un mauvais nom (AC3 rajouté)
    public class CSP5DefaultOrderLCVAC3Solver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = true,
                Inference = CSPInference.AC3,
                Selection = CSPSelection.DefaultOrder,
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy,
                MaxSteps = 1000
            };
            return objStrategyInfo.GetStrategy();
        }
    }

    //Ce solver était en double, je le hijack pour tester le backtracking simple
    public class CSP6BacktrackingSolver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = true,
                Inference = CSPInference.ForwardChecking,
                Selection = CSPSelection.MRVDeg,
                StrategyType = CSPStrategy.BacktrackingStrategy,
                MaxSteps = 1000
			};
            return objStrategyInfo.GetStrategy();
        }
    }

    //A priori le paramètre maxsteps ne concerne que le solver minconflicts
    public class CSP7MinConflicts10000Solver : CSPSolverBase
    {
        protected override SolutionStrategy GetStrategy()
        {
            var objStrategyInfo = new CSPStrategyInfo
            {
                EnableLCV = true,
                Inference = CSPInference.ForwardChecking,
                Selection = CSPSelection.MRVDeg,
                StrategyType = CSPStrategy.ImprovedBacktrackingStrategy,
                MaxSteps = 10000
            };
            return objStrategyInfo.GetStrategy();
        }
    }

}