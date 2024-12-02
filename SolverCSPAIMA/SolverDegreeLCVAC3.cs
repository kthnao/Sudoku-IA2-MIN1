namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeLCVAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("degree", "lcv", "ac3");  // Combinaison Degree + LCV + AC3
        }
    }
}
