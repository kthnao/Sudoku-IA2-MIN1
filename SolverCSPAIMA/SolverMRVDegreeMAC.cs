namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts) GetStrategies()
        {
            return ("mrv", "degree", "mac",false);  // Combinaison MRV + degree + MAC
        }
    }
}
