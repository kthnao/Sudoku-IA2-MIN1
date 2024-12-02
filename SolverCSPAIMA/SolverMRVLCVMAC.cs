namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVLCVMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts) GetStrategies()
        {
            return ("mrv", "lcv", "mac",false);  // Combinaison MRV + LCV + MAC
        }
    }
}
