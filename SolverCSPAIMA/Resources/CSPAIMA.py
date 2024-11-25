import numpy as np
from aima3.logic import *
from aima3.csp import *

# Définir les variables, le domaine et les contraintes pour le Sudoku
def sudoku_variables():
    return [(r, c) for r in range(9) for c in range(9)]

def sudoku_domains():
    return {var: list(range(1, 10)) if instance[var[0], var[1]] == 0 else [instance[var[0], var[1]]] for var in sudoku_variables()}

def sudoku_constraints():
    # Les contraintes de ligne, de colonne et de bloc 3x3
    constraints = []
    for r in range(9):
        for c in range(9):
            var = (r, c)
            row = [(r, j) for j in range(9)]
            col = [(i, c) for i in range(9)]
            block = [(i, j) for i in range(3 * (r // 3), 3 * (r // 3) + 3)
                          for j in range(3 * (c // 3), 3 * (c // 3) + 3)]
            # Ajouter les contraintes de ligne, colonne et bloc
            constraints.append(AllDiff(row))
            constraints.append(AllDiff(col))
            constraints.append(AllDiff(block))
    return constraints

# Convertir l'instance du Sudoku en un problème CSP
def create_sudoku_csp(instance):
    # Définir les variables, les domaines et les contraintes
    variables = sudoku_variables()
    domains = sudoku_domains()
    constraints = sudoku_constraints()

    # Créer un problème CSP
    csp = CSP(variables, domains)
    for constraint in constraints:
        csp.add_constraint(constraint)
    
    return csp

# Résoudre le Sudoku en utilisant CSP et Backtracking Search
def solve_sudoku_with_csp(instance):
    csp = create_sudoku_csp(instance)
    solution = backtrack(csp)
    return solution

# Exemple d'instance de Sudoku (0 représente les cases vides)
if 'instance' in locals():
    result = solve_sudoku_with_csp(instance)
else:
    result = None

