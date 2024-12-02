namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVLCVNoInference : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "lcv", "ni");  // Combinaison MRV + LCV + No Inference
        }
    }
}
