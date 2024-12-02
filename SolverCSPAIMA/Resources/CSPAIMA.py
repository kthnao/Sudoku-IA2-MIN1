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

def degree(assignment, csp):
    """Retourne la variable avec le plus grand nombre de voisins non assignés."""
    unassigned_vars = [var for var in csp.variables if var not in assignment]
    return max(unassigned_vars, key=lambda var: len([n for n in csp.neighbors[var] if n not in assignment]))

def mrv_degree(assignment, csp):
    """Retourne la variable avec le moins de valeurs restantes, puis le plus grand nombre de voisins non assignés"""
    unassigned_vars = [var for var in csp.variables if var not in assignment]
    mrv_vars = [var for var in unassigned_vars if len(csp.domains[var]) == min(len(csp.domains[v]) for v in unassigned_vars)]
    return max(mrv_vars, key=lambda var: len([n for n in csp.neighbors[var] if n not in assignment]))

def create_neighbors():
    """Optimisation de la création des voisins dans une grille de Sudoku."""
    neighbors = { (i, j): set() for i in range(9) for j in range(9) }
    
    for i in range(9):
        for j in range(9):
            # Ajouter les voisins dans la même ligne
            for col in range(9):
                if col != j:
                    neighbors[(i, j)].add((i, col))
            
            # Ajouter les voisins dans la même colonne
            for row in range(9):
                if row != i:
                    neighbors[(i, j)].add((row, j))
            
            # Ajouter les voisins dans le même carré 3x3
            start_row, start_col = 3 * (i // 3), 3 * (j // 3)
            for r in range(start_row, start_row + 3):
                for c in range(start_col, start_col + 3):
                    if (r, c) != (i, j):
                        neighbors[(i, j)].add((r, c))

    return neighbors

def min_conflicts_sudoku(board, max_steps=10000):
    # Algorithme Min Conflicts pour résoudre le Sudoku
    for _ in range(max_steps):
        var = select_variable_with_max_conflict(board)  # Choisir la variable avec le plus de conflits
        if var is None:  # Si aucune variable n'a de conflits (grille complète)
            return board
        row, col = var
        
        # Choisir une valeur qui minimise les conflits pour cette cellule
        new_value = choose_value_with_min_conflict(board, row, col)
        board[row][col] = new_value
    
    return None  # Retourne None si la solution n'est pas trouvée après max_steps

def solve_min_conflicts_csp(grid):
    """Résoudre le Sudoku avec l'algorithme Min Conflicts"""
    solved_grid = min_conflicts_sudoku(grid, max_steps=10000)
    return solved_grid


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
    "mac": mac,  # Maintain Arc-Consistency
    "min_conflicts": min_conflicts_sudoku  # Ajoute Min Conflicts ici
}


def solve_sudoku_csp(grid, variable_heuristic, value_order, inference_method, use_min_conflict):
    """Résoudre le Sudoku avec les heuristiques et stratégies passées"""
    
    # Afficher la grille d'entrée pour vérifier qu'elle est bien reçue
    print("Grille initiale:")
    print(np.array(grid))  # Afficher la grille sous forme de tableau NumPy

    # Initialiser les variables et domaines pour CSP
    variables = [(i, j) for i in range(9) for j in range(9)]
    domains = {(i, j): [grid[i][j]] if grid[i][j] != 0 else list(range(1, 10)) for i in range(9) for j in range(9)}

    # Initialisation des voisins
    neighbors = create_neighbors()

    # Créer un CSP
    csp = CSP(variables, domains, neighbors, sudoku_constraint)

    # Appliquer les heuristiques et stratégies passées
    if use_min_conflict:
        # Si Min Conflicts est activé, on l'utilise pour résoudre
        solved_grid = min_conflicts_sudoku(grid)
    else:
        # Sinon, on applique les heuristiques et stratégies habituelles
        solved_grid = backtracking_search(
            csp,
            select_unassigned_variable=heuristics_dict[variable_heuristic],  # Mapper à la fonction
            order_domain_values=value_orders_dict[value_order],  # Mapper à la fonction
            inference=inference_methods_dict[inference_method]  # Mapper à la fonction
        )


    solved_grid = np.zeros((9, 9), dtype=int)

    if solved_grid is None:
        print("Failed to solve the Sudoku puzzle")
    
    else:
        # Créer la grille résolue        
        for i in range(solved_grid.shape[0]):  # Parcours des lignes
            for j in range(solved_grid.shape[1]):  # Parcours des colonnes
                solved_grid[i, j] = solved_grid[i, j]


    return solved_grid


# Appel de la fonction avec les arguments passés depuis C#
solved_grid = solve_sudoku_csp(instance, variable_heuristic, value_order, inference_method, use_min_conflict)

# Afficher la grille résolue
print("\nGrille résolue:")
print(solved_grid)
