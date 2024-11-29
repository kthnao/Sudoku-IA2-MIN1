import numpy as np
from aima.core.search.csp import CSP, Domain, Variable, Constraint, Assignment
import re
from collections import deque


class SudokuCSP:

    _BaseSudokuCSP = None
    _BaseSudokuCSPLock = object()
    _NameRegex = re.compile(r"cell(?P<row>\d)(?P<col>\d)")

    @staticmethod
    def GetSudokuCSP(sudoku_grid):
        """
        Initialise un CSP pour le Sudoku en fonction de la grille fournie.
        :param sudoku_grid: La grille de Sudoku à résoudre (tableau 9x9)
        :return: CSP avec les contraintes et variables
        """
        to_return = SudokuCSP.GetSudokuBaseCSP()

        # Ajout des valeurs fixes dans la grille
        mask = {}
        for i in range(9):
            for j in range(9):
                cell_val = sudoku_grid[i][j]
                if cell_val != 0:
                    mask[i * 9 + j] = cell_val

        cell_vars = to_return.getVariables()

        mask_queue = deque(mask.keys())

        # Mise à jour des domaines des variables avec les valeurs du masque
        while mask_queue:
            current_mask_idx = mask_queue.popleft()
            current_var_name = SudokuCSP.GetVarName(current_mask_idx // 9, current_mask_idx % 9)

            for obj_var in cell_vars:
                if obj_var.getName() == current_var_name:
                    cell_value = mask[current_mask_idx]
                    to_return.setDomain(obj_var, Domain([cell_value]))
                    if not mask_queue:
                        break
                    current_mask_idx = mask_queue.popleft()

        return to_return

    @staticmethod
    def SetValuesFromAssignment(assignment, sudoku_grid):
        """
        Met à jour la grille de Sudoku avec les valeurs obtenues dans l'assignation.
        :param assignment: L'assignation des variables
        :param sudoku_grid: La grille de Sudoku à mettre à jour
        """
        for obj_var in assignment.getVariables():
            row_idx, col_idx = SudokuCSP.GetIndices(obj_var)
            value = assignment.getAssignment(obj_var)
            sudoku_grid[row_idx][col_idx] = value

    @staticmethod
    def GetSudokuBaseCSP():
        """
        Crée la base du CSP pour le Sudoku, incluant les variables et les contraintes.
        :return: CSP avec les variables et les contraintes de base
        """
        if SudokuCSP._BaseSudokuCSP is None:
            with SudokuCSP._BaseSudokuCSPLock:
                if SudokuCSP._BaseSudokuCSP is None:
                    to_return = DynamicCSP()

                    # Domaine des variables
                    cell_possible_values = list(range(1, 10))
                    cell_domain = Domain(cell_possible_values)

                    # Variables
                    variables = {}
                    for row_index in range(9):
                        row_vars = {}
                        for col_index in range(9):
                            var_name = SudokuCSP.GetVarName(row_index, col_index)
                            cell_variable = Variable(var_name)
                            to_return.AddNewVariable(cell_variable)
                            to_return.setDomain(cell_variable, cell_domain)
                            row_vars[col_index] = cell_variable
                        variables[row_index] = row_vars

                    # Contraintes (lignes, colonnes, boîtes)
                    constraints = []

                    # Lignes
                    for row_vars in variables.values():
                        ligne_constraints = SudokuCSP.GetAllDiffConstraints(list(row_vars.values()))
                        constraints.extend(ligne_constraints)

                    # Colonnes
                    for j in range(9):
                        col_vars = [variables[i][j] for i in range(9)]
                        col_constraints = SudokuCSP.GetAllDiffConstraints(col_vars)
                        constraints.extend(col_constraints)

                    # Boîtes
                    for b in range(9):
                        boite_vars = []
                        i_start = 3 * (b // 3)
                        j_start = 3 * (b % 3)
                        for i in range(3):
                            for j in range(3):
                                boite_vars.append(variables[i_start + i][j_start + j])
                        boite_constraints = SudokuCSP.GetAllDiffConstraints(boite_vars)
                        constraints.extend(boite_constraints)

                    # Ajouter les contraintes
                    for constraint in constraints:
                        to_return.addConstraint(constraint)

                    SudokuCSP._BaseSudokuCSP = to_return

        return SudokuCSP._BaseSudokuCSP.clone()

    @staticmethod
    def GetIndices(variable):
        """
        Récupère les indices (ligne, colonne) à partir du nom de la variable.
        :param variable: La variable du CSP
        :return: Indices (ligne, colonne)
        """
        match = SudokuCSP._NameRegex.match(variable.getName())
        row_idx = int(match.group("row"))
        col_idx = int(match.group("col"))
        return row_idx, col_idx

    @staticmethod
    def GetVarName(row_index, col_index):
        """
        Génère le nom d'une variable pour la case (row_index, col_index).
        :param row_index: Indice de la ligne
        :param col_index: Indice de la colonne
        :return: Nom de la variable (cellXXYY)
        """
        return f"cell{row_index}{col_index}"

    @staticmethod
    def GetAllDiffConstraints(vars):
        """
        Génère des contraintes d'inégalité entre toutes les variables d'une liste.
        :param vars: Liste des variables
        :return: Liste des contraintes d'inégalité
        """
        constraints = []
        for i in range(len(vars)):
            for j in range(i + 1, len(vars)):
                diff_constraint = NotEqualConstraint(vars[i], vars[j])
                constraints.append(diff_constraint)
        return constraints


# Classe DynamicCSP pour simuler la version C# (si nécessaire en Python)
class DynamicCSP(CSP):
    def __init__(self):
        super().__init__()

    def AddNewVariable(self, variable):
        self.addVariable(variable)

    def clone(self):
        to_return = DynamicCSP()
        for variable in self.getVariables():
            to_return.addVariable(variable)
            to_return.setDomain(variable, Domain(self.getDomain(variable).asList()))
        for constraint in self.getConstraints():
            to_return.addConstraint(constraint)
        return to_return


# Classe NotEqualConstraint pour les contraintes d'inégalité
class NotEqualConstraint(Constraint):
    def __init__(self, var1, var2):
        super().__init__([var1, var2])

    def isSatisfiedWith(self, assignment):
        return assignment.getAssignment(self.getScope()[0]) != assignment.getAssignment(self.getScope()[1])


# Fonction de résolution du Sudoku en CSP
def solve_sudoku_csp(sudoku_grid):
    csp = SudokuCSP.GetSudokuCSP(sudoku_grid)

    # Exécution de la recherche avec backtracking (et d'autres heuristiques comme MRV)
    solution = backtracking_search(csp, select_unassigned_variable=mrv, order_domain_values=lcv, inference=mac)

    # Vérification et retour de la solution
    solved_grid = np.zeros((9, 9), dtype=int)
    if solution:
        SudokuCSP.SetValuesFromAssignment(solution, solved_grid)
    
    return solved_grid

