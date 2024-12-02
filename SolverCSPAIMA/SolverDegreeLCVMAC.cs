namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeLCVMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("degree", "lcv", "mac");  // Combinaison Degree + LCV + MAC
        }
    }
}
