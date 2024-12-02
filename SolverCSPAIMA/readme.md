Projet Sudoku Solver avec CSP AIMA

Approche
Notre projet utilise les CSP (problèmes de satisfaction de contraintes) implémentés dans le framework AIMA3 pour résoudre des grilles de Sudoku. Nous avons utilisé le langage python. Nous avons expérimenté différentes heuristiques pour optimiser la résolution des sudoku en jouant sur trois caractéristiques principales :

Sélection des variables :

FUV (First Unassigned Variable) : Sélectionne la première variable non assignée.
MRV (Minimum Remaining Values) : Priorise les variables ayant le moins de valeurs possibles dans leur domaine.
Ordres de choix des valeurs :

LCV (Least Constraining Value) : Choisit les valeurs qui restreignent le moins les autres variables.
UDV (Unordered Domain Values) : Explore les valeurs dans l’ordre naturel ou de manière aléatoire.
Méthodes d’inférence :

NI (No Inference) : Ne réalise aucune inférence pendant la recherche.
FC (Forward Checking) : Élimine les valeurs incohérentes dans les domaines des variables liées.
MAC (Maintien de la cohérence des arcs) : Vérifie et maintient la cohérence des arcs entre les variables.
AC3 (Arc Consistency 3) : Réduit les domaines avant et pendant la recherche via la propagation des contraintes.