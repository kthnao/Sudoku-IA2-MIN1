import numpy as np
from aima3.logic import *
from aima3.csp import backtracking_search, AC3, mrv, lcv, mac, CSP
import random

# Fonction de validation des contraintes (les cases adjacentes doivent avoir des valeurs différentes)
def sudoku_constraint(A, a, B, b):
    """Les cases adjacentes (A, B) doivent respecter les règles du Sudoku :
       1. Pas de doublon dans une ligne.
       2. Pas de doublon dans une colonne.
       3. Pas de doublon dans un carré 3x3."""
    
    # Extraire les coordonnées des variables
    i, j = A
    k, l = B
    
    # 1. Pas de doublon dans la ligne
    if i == k and a == b:
        return False
    # 2. Pas de doublon dans la colonne
    if j == l and a == b:
        return False
    # 3. Pas de doublon dans la sous-grille 3x3
    if (i // 3 == k // 3) and (j // 3 == l // 3) and a == b:
        return False
    
    return True

def solve_sudoku_csp(grid, variable_heuristic, value_order, inference_method):
    """Résoudre le Sudoku avec les heuristiques et stratégies passées"""
    
    # Afficher la grille d'entrée pour vérifier qu'elle est bien reçue
    print("Grille initiale:")
    print(np.array(grid))  # Afficher la grille sous forme de tableau NumPy

    # Initialiser les variables et domaines pour CSP
    variables = [(i, j) for i in range(9) for j in range(9)]
    domains = {(i, j): [grid[i][j]] if grid[i][j] != 0 else list(range(1, 10)) for i in range(9) for j in range(9)}

    # Initialiser les voisins
    neighbors = {}
    for i in range(9):
        for j in range(9):
            neighbors[(i, j)] = set()
            for x in range(9):
                if x != i:
                    neighbors[(i, j)].add((x, j))  # Voisin dans la même colonne
                if x != j:
                    neighbors[(i, j)].add((i, x))  # Voisin dans la même ligne
            start_row, start_col = 3 * (i // 3), 3 * (j // 3)
            for r in range(start_row, start_row + 3):
                for c in range(start_col, start_col + 3):
                    if (r, c) != (i, j):
                        neighbors[(i, j)].add((r, c))

    # Créer un CSP
    csp = CSP(variables, domains, neighbors, sudoku_constraint)

    # Appliquer les heuristiques et stratégies passées
    solution = backtracking_search(
        csp,
        select_unassigned_variable=heuristics_dict[variable_heuristic],  # Mapper à la fonction
        order_domain_values=value_orders_dict[value_order],  # Mapper à la fonction
        inference=inference_methods_dict[inference_method]  # Mapper à la fonction
    )

    solved_grid = np.zeros((9, 9), dtype=int)

    if solution is None:
        print("Failed to solve the Sudoku puzzle")
    
    else:
        # Créer la grille résolue        
        for (i, j), value in solution.items():
            solved_grid[i, j] = value

    return solved_grid

# Block de code à fins de débogage
if __name__ == "__main__":
    # Tableau NumPy représentant une grille de Sudoku
    instance = np.array([
    [4, 0, 0, 0, 0, 0, 8, 0, 5],
    [0, 3, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 7, 0, 0, 0, 0, 0],
    [0, 2, 0, 0, 0, 0, 0, 6, 0],
    [0, 0, 0, 0, 8, 0, 4, 0, 0],
    [0, 0, 0, 0, 1, 0, 0, 0, 0],
    [0, 0, 0, 6, 0, 3, 0, 7, 0],
    [5, 0, 0, 2, 0, 0, 0, 0, 0],
    [1, 0, 4, 0, 0, 0, 0, 0, 0]
    ])
    variable_heuristic = "mrv"
    value_order = "lcv"
    inference_method = "mac"


# Appel de la fonction avec les arguments passés depuis C#
solved_grid = solve_sudoku_csp(instance, variable_heuristic, value_order, inference_method)

# Afficher la grille résolue
print("\nGrille résolue:")
print(solved_grid)
