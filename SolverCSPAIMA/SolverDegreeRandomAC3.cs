namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeRandomAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod, bool useMinConflicts) GetStrategies()
        {
            return ("degree", "random", "ac3",false);  // Combinaison Degree + Random + AC3
        }
    }
}
