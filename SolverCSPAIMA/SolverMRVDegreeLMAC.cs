namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeLMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv_degree", "lcv", "mac");  // Combinaison MRV + degree + LCV + MAC
        }
    }
}
