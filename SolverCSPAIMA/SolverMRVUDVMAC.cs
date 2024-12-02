namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVUDVMAC: SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "udv", "mac");  // Combinaison MRV + UDV + MAC
        }
    }
}
