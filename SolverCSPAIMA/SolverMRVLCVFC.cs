namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVLCVFC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "lcv", "fc");  // Combinaison MRV + LCV + Forward Checking
        }
    }
}
