namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVRandomAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts) GetStrategies()
        {
            return ("mrv", "random", "ac3",false);  // Combinaison MRV + random + AC3
        }
    }
}
