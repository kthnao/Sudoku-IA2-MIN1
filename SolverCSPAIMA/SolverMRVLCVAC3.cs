namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVLCVAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "lcv", "ac3");  // Combinaison MRV + LCV + AC3
        }
    }
}
