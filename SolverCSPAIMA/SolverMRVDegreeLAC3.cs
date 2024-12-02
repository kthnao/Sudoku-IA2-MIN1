namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeLAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv_degree", "lcv", "ac3");  // Combinaison MRV + Degree + LCV + AC3
        }
    }
}
