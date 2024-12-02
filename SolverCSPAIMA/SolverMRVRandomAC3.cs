namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVRandomAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "random", "ac3");  // Combinaison MRV + Random + AC3
        }
    }
}
