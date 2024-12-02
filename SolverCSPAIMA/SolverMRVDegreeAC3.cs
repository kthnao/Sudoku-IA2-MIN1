namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv_degree", "random", "ac3");  // Combinaison MRV + Degree + AC3
        }
    }
}
