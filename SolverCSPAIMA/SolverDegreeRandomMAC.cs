namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeRandomMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("degree", "random", "mac");  // Combinaison Degree + Random + MAC
        }
    }
}
