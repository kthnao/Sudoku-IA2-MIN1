namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeLCVMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts ) GetStrategies()
        {
            return ("degree", "lcv", "mac",false);  // Combinaison Degree + LCV + MAC
        }
    }
}
