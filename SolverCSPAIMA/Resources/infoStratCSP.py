from aima.core.search.csp import BacktrackingSolver, MinConflictsSolver, AC3, MRV, LCV, ForwardChecking

class infoStratCSP:
    def __init__(self):
        self.strategy_type = None  # Type de stratégie (Backtracking, ImprovedBacktracking, etc.)
        self.selection = None       # Sélection de variable (DefaultOrder, MRV, MRVDeg)
        self.inference = None       # Méthode d'inférence (None, ForwardChecking, AC3)
        self.enable_lcv = False     # Activation de LCV (Least Constraining Value)
        self.max_steps = 50         # Maximum d'étapes pour Min Conflicts

    def get_strategy(self):
        if self.strategy_type == "BacktrackingStrategy":
            solver = BacktrackingSolver()
            return solver

        elif self.strategy_type == "ImprovedBacktrackingStrategy":
            solver = BacktrackingSolver()

            # Configuration de la sélection de variables
            if self.selection == "MRV":
                solver.set_variable_selection(MRV())
            elif self.selection == "MRVDeg":
                solver.set_variable_selection(MRV(), degree_heuristic=True)  # MRVDeg combinaison

            # Configuration de l'inférence
            if self.inference == "ForwardChecking":
                solver.set_inference(ForwardChecking())
            elif self.inference == "AC3":
                solver.set_inference(AC3())

            # Activation de LCV
            if self.enable_lcv:
                solver.set_value_ordering(LCV())

            return solver

        elif self.strategy_type == "MinConflictsStrategy":
            solver = MinConflictsSolver(self.max_steps)
            return solver

        else:
            raise ValueError("Invalid strategy type selected")
