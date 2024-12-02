namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts) GetStrategies()
        {
            return ("mrv_degree", "random", "ac3",false);  // Combinaison MRV + Degree + AC3
        }
    }
}
