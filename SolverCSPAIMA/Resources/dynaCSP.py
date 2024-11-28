from aima.core.search.csp import CSP, Variable, Domain, Constraint
from copy import deepcopy

class DynaCSP(CSP):
    def __init__(self):
        super().__init__()

    def add_new_variable(self, obj_variable: Variable):
        # Ajoute une variable au CSP
        self.add_variable(obj_variable)

    def clone(self):
        # Clone l'objet CSP, copiant variables et contraintes
        to_return = DynamicCSP()
        
        # Copie des variables et domaines
        for variable in self.get_variables():
            to_return.add_variable(variable)
            to_return.set_domain(variable, Domain(self.get_domain(variable).as_list()))
        
        # Copie des contraintes
        for constraint in self.get_constraints():
            to_return.add_constraint(constraint)
        
        return to_return
