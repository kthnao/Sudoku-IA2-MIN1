from aima.core.search.csp import CSPStateListener, Assignment, CSP
from collections import defaultdict

class StepCount(CSPStateListener):
    def __init__(self):
        self.assignment_count = 0
        self.domain_count = 0

    def state_changed(self, assignment, csp):
        # Incrémenter le nombre d'assignations
        self.assignment_count += 1

    def state_changed_no_assignment(self, csp):
        # Incrémenter le nombre de changements de domaine
        self.domain_count += 1

    def reset(self):
        # Réinitialiser les compteurs
        self.assignment_count = 0
        self.domain_count = 0

    def get_results(self):
        # Retourner les résultats sous forme de chaîne
        results = f"assignment changes: {self.assignment_count}"
        if self.domain_count != 0:
            results += f", domain changes: {self.domain_count}"
        return results

    def __str__(self):
        return self.get_results()
