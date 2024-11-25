import numpy as np
from aima3.logic import *

# Fonction de validation des contraintes
def sudoku_constraint(A, a, B, b):
    return a != b  # Les valeurs des voisins doivent être différentes

# Résolution du Sudoku en utilisant CSP
def solve_sudoku_csp(grid):
    variables = [(i, j) for i in range(9) for j in range(9)]
    domains = {(i, j): [grid[i][j]] if grid[i][j] != 0 else list(range(1, 10)) for i in range(9) for j in range(9)}

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

    csp = CSP(variables, domains, neighbors, sudoku_constraint)
    solution = backtracking_search(csp, inference=AC3)

    solved_grid = np.zeros((9, 9), dtype=int)
    for (i, j), value in solution.items():
        solved_grid[i, j] = value
    
    return solved_grid.tolist()
