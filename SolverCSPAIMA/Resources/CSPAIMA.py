import numpy as np
from aima3.logic import *
from aima3.csp import backtracking_search, AC3, mrv, lcv, mac, CSP
import random
import threading

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
    unassigned_vars = [var for var in csp.variables if var not in assignment]
    if not unassigned_vars:
        return None  # Toutes les variables sont assignées
    return max(unassigned_vars, key=lambda var: sum(1 for n in csp.neighbors[var] if n not in assignment))

def mrv_degree(assignment, csp):
    unassigned_vars = [var for var in csp.variables if var not in assignment]
    if not unassigned_vars:
        return None  # Toutes les variables sont assignées
    
    # Trouver la valeur minimale du domaine sans recalculer à chaque itération
    min_domain_size = min(len(csp.domains[var]) for var in unassigned_vars)
    mrv_vars = [var for var in unassigned_vars if len(csp.domains[var]) == min_domain_size]
    
    # Appliquer l'heuristique du degré pour départager les variables MRV
    return max(mrv_vars, key=lambda var: sum(1 for n in csp.neighbors[var] if n not in assignment))

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
}

def solve_sudoku_csp_with_timeout(grid, variable_heuristic, value_order, inference_method, timeout=60):
    """Résoudre le Sudoku avec une limite de temps."""
    
    # Créer un tableau pour stocker la solution
    solved_grid = np.zeros((9, 9), dtype=int)
    
    # Drapeau pour indiquer si la solution a été trouvée dans les délais
    result_found = threading.Event()

    def solve():
        nonlocal solved_grid
        
        # Afficher la grille d'entrée
        print("Grille initiale:")
        print(np.array(grid))

        # Initialiser les variables et domaines pour CSP
        variables = [(i, j) for i in range(9) for j in range(9)]
        domains = {(i, j): [grid[i][j]] if grid[i][j] != 0 else list(range(1, 10)) for i in range(9) for j in range(9)}
        neighbors = create_neighbors()
        
        # Créer un CSP
        csp = CSP(variables, domains, neighbors, sudoku_constraint)
        
        # Appliquer les heuristiques et stratégies passées
        solution = backtracking_search(
            csp,
            select_unassigned_variable=heuristics_dict[variable_heuristic],
            order_domain_values=value_orders_dict[value_order],
            inference=inference_methods_dict[inference_method]
        )
        
        if solution:
            # Convertir la solution en grille NumPy
            for (i, j), value in solution.items():
                solved_grid[i, j] = value
            result_found.set()  # Indiquer que la solution a été trouvée

    # Lancer le solveur dans un thread séparé
    solver_thread = threading.Thread(target=solve)
    solver_thread.start()
    
    # Attendre que le solveur se termine ou que le délai expire
    solver_thread.join(timeout)
    
    # Si le solveur n'a pas fini dans le temps imparti
    if not result_found.is_set():
        print("Temps limite dépassé. Retour d'une grille vide.")
        return np.zeros((9, 9), dtype=int)
    
    print("\nGrille résolue:")
    return solved_grid


# Appel de la fonction avec les arguments passés depuis C#
solved_grid = solve_sudoku_csp_with_timeout(instance, variable_heuristic, value_order, inference_method)

# Afficher la grille résolue
print("\nGrille résolue:")
print(solved_grid)
