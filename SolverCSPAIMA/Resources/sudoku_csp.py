import re
from collections import deque
from aima.core.search.csp import CSP, Domain, Variable, Constraint, Assignment

class SudokuCSPHelper:
    _BaseSudokuCSP = None
    _BaseSudokuCSPLock = object()
    _NameRegex = re.compile(r"cell(?P<row>\d)(?P<col>\d)")

    @staticmethod
    def GetSudokuCSP(sudoku_grid):
        to_return = SudokuCSPHelper.GetSudokuBaseCSP()

        mask = {}
        for i in range(9):
            for j in range(9):
                cell_val = sudoku_grid[i][j]
                if cell_val != 0:
                    mask[i * 9 + j] = cell_val

        mask_queue = deque(mask.keys())
        cell_vars = to_return.getVariables()

        while mask_queue:
            current_mask_idx = mask_queue.popleft()
            current_var_name = SudokuCSPHelper.GetVarName(current_mask_idx // 9, current_mask_idx % 9)

            for obj_var in cell_vars:
                if obj_var.getName() == current_var_name:
                    cell_value = mask[current_mask_idx]
                    to_return.setDomain(obj_var, Domain([cell_value]))

                    if not mask_queue:
                        break
                    current_mask_idx = mask_queue.popleft()

        return to_return

    @staticmethod
    def GetSudokuBaseCSP():
        if SudokuCSPHelper._BaseSudokuCSP is None:
            with SudokuCSPHelper._BaseSudokuCSPLock:
                if SudokuCSPHelper._BaseSudokuCSP is None:
                    to_return = DynamicCSP()

                    cell_possible_values = list(range(1, 10))
                    cell_domain = Domain(cell_possible_values)

                    variables = {}
                    for row_index in range(9):
                        row_vars = {}
                        for col_index in range(9):
                            var_name = SudokuCSPHelper.GetVarName(row_index, col_index)
                            cell_variable = Variable(var_name)
                            to_return.AddNewVariable(cell_variable)
                            to_return.setDomain(cell_variable, cell_domain)
                            row_vars[col_index] = cell_variable
                        variables[row_index] = row_vars

                    constraints = []

                    # Ajouter des contraintes de lignes, colonnes et boîtes
                    constraints.extend(SudokuCSPHelper.AddConstraints(variables))

                    for constraint in constraints:
                        to_return.addConstraint(constraint)

                    SudokuCSPHelper._BaseSudokuCSP = to_return

        return SudokuCSPHelper._BaseSudokuCSP.clone()

    @staticmethod
    def AddConstraints(variables):
        constraints = []

        # Lignes
        for row_vars in variables.values():
            ligne_constraints = SudokuCSPHelper.GetAllDiffConstraints(list(row_vars.values()))
            constraints.extend(ligne_constraints)

        # Colonnes
        for j in range(9):
            col_vars = [variables[i][j] for i in range(9)]
            col_constraints = SudokuCSPHelper.GetAllDiffConstraints(col_vars)
            constraints.extend(col_constraints)

        # Boîtes
        for b in range(9):
            boite_vars = []
            i_start = 3 * (b // 3)
            j_start = 3 * (b % 3)
            for i in range(3):
                for j in range(3):
                    boite_vars.append(variables[i_start + i][j_start + j])
            boite_constraints = SudokuCSPHelper.GetAllDiffConstraints(boite_vars)
            constraints.extend(boite_constraints)

        return constraints

    @staticmethod
    def GetAllDiffConstraints(vars):
        constraints = []
        for i in range(len(vars)):
            for j in range(i + 1, len(vars)):
                diff_constraint = NotEqualConstraint(vars[i], vars[j])
                constraints.append(diff_constraint)
        return constraints

    @staticmethod
    def GetVarName(row_index, col_index):
        return f"cell{row_index}{col_index}"


class NotEqualConstraint(Constraint):
    def __init__(self, var1, var2):
        super().__init__([var1, var2])

    def isSatisfiedWith(self, assignment):
        return assignment.getAssignment(self.getScope()[0]) != assignment.getAssignment(self.getScope()[1])


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
