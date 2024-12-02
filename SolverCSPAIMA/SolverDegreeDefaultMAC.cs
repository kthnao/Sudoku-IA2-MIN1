namespace Sudoku.SolverCSPAIMA
{
    public class SolverDegreeDefaultMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("degree", "default", "mac");  // Combinaison Degree + Default + MAC
        }
    }
}
