namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVLCVMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "lcv", "mac");  // Combinaison MRV + LCV + MAC
        }
    }
}
