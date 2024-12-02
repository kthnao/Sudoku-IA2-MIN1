namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "degree", "mac");  // Combinaison MRV + degree + MAC
        }
    }
}
