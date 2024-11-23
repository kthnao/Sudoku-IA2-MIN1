from aima3.logic import *
from aima3.search import *
import numpy as np

class SudokuCSP(Problem):
    def __init__(self, grid):
        self.grid = grid
        self.variables = [(row, col) for row in range(9) for col in range(9) if grid[row][col] == 0]
        self.domains = {var: list(range(1, 10)) for var in self.variables}
        self.constraints = []

    def actions(self, state):
        # Retourne les valeurs possibles pour une variable donnée
        for var in self.variables:
            if state[var] == 0:
                return [(var, value) for value in self.domains[var]]
        return []

    def result(self, state, action):
        var, value = action
        new_state = state.copy()
        new_state[var] = value
        return new_state

    def goal_test(self, state):
        return all(state[var] != 0 for var in self.variables) and self.check_validity(state)

    def check_validity(self, state):
        # Vérifie les contraintes de lignes, colonnes et sous-grilles
        for row in range(9):
            if not self.valid_set([state[row, col] for col in range(9)]):
                return False
        for col in range(9):
            if not self.valid_set([state[row, col] for row in range(9)]):
                return False
        for row in range(0, 9, 3):
            for col in range(0, 9, 3):
                if not self.valid_set([state[row+i, col+j] for i in range(3) for j in range(3)]):
                    return False
        return True

def solve_sudoku(grid):
    problem = SudokuCSP(grid)
    return backtracking_search(problem)
