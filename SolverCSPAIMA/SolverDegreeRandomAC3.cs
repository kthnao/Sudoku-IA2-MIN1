namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeRandomAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("degree", "random", "ac3");  // Combinaison Degree + Random + AC3
        }
    }
}
