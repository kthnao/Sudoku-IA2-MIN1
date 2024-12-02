namespace Sudoku.SolverCSPAIMA
{
    public class SolverFUVDefaultFC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("fuv", "default", "fc");  // Combinaison FUV + Default + Forward Checking
        }
    }
}
