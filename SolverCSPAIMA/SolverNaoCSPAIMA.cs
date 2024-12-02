using Python.Runtime;
using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.SolverCSPAIMA
{
   public class SolverNaoCSPAIMA : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            // Spécifiez ici les combinaisons de stratégies que vous souhaitez tester
            return ("mrv", "lcv", "mac");
        }
    }
}
