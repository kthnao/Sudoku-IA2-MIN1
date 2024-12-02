using Python.Runtime;
using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku.SolverCSPAIMA
{
   public class SolverNaoCSPAIMA : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            // Spécifiez ici les combinaisons de stratégies que vous souhaitez tester
            return ("mrv", "lcv", "mac");
        }
    }

    /*def degree(assignment, csp):
    """Retourne la variable avec le plus grand nombre de voisins non assignés."""
    unassigned_vars = [var for var in csp.variables if var not in assignment]
    return max(unassigned_vars, key=lambda var: len([n for n in csp.neighbors[var] if n not in assignment]))

def mrv_degree(assignment, csp):
    """Retourne la variable avec le moins de valeurs restantes, puis le plus grand nombre de voisins non assignés"""
    unassigned_vars = [var for var in csp.variables if var not in assignment]
    mrv_vars = [var for var in unassigned_vars if len(csp.domains[var]) == min(len(csp.domains[v]) for v in unassigned_vars)]
    return max(mrv_vars, key=lambda var: len([n for n in csp.neighbors[var] if n not in assignment]))

# Dictionnaire pour maper les stratégies en fonctions
heuristics_dict = {
    "mrv": mrv,  # Minimum Remaining Values
    "degree": degree,  # Degree heuristic
    "mrv_degree": mrv_degree  # Minimum Remaining Values + Degree heuristic
}

value_orders_dict = {
    "lcv": lcv,  # Least Constraining Value
    "random": lambda var, assignment, csp: sorted(csp.choices(var), key=lambda _: random.random())
}

inference_methods_dict = {
    "ac3": lambda csp, var, value, assignment, removals: AC3(csp),  # Corrigé pour que AC3 soit utilisé correctement dans le contexte
    "mac": mac  # Maintain Arc-Consistency
}*/
}
