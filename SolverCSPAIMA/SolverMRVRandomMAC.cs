namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVRandomMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts) GetStrategies()
        {
            return ("mrv", "random", "mac",false);  // Combinaison MRV + random + MAC
        }
    }
}
