namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeRMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv_degree", "random", "mac");  // Combinaison MRV + degree + random + MAC
        }
    }
}
