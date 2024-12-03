# Solveur de Sudoku : Recuit Simulé
Ce projet propose un solveur probabiliste de Sudoku basé sur un algorithme Recuit Simulé.<br>
L'objectif est de trouver une solution valide à un puzzle Sudoku tout en minimisant les erreurs à chaque étape.<br>
Cet algorithme utilise une approche stochastique et exploite un mécanisme de refroidissement simulé pour converger vers une solution optimale.

## 1. Vue d'ensemble
Objectif
Le solveur implémente un algorithme probabiliste pour résoudre des puzzles Sudoku. 
Il respecte l'ensemble des règles classiques du Sudoku :

Les lignes, colonnes et sous-grilles 3x3 doivent contenir des chiffres uniques de 1 à 9.
Le solveur s'appuie sur les principes suivants :

Génération aléatoire de solutions initiales tout en respectant les valeurs fixes du puzzle.<br>
Évaluation du nombre d'erreurs dans la solution proposée (contraventions aux règles du Sudoku).<br>
Modification progressive de la grille en cherchant à réduire les erreurs, en appliquant une probabilité décroissante d'accepter des solutions moins bonnes (selon un facteur de température, sigma).<br>
**Caractéristiques**<br>
Optimisation par annealing simulé pour explorer l'espace des solutions.<br>
Réduction progressive du facteur de température pour affiner la solution.<br>
Capacité à gérer des puzzles de complexité variable, en adaptant la fréquence des itérations en fonction des contraintes du puzzle.<br>
## 2. Structure du code
### 2.1 Classes et fonctions principales
- **PrintSudoku(sudoku)** <br>
Affiche une représentation lisible de la grille Sudoku actuelle.

- **FixSudokuValues(sudoku)** <br>
Identifie et marque les cases fixes dans la grille initiale (valeurs données du puzzle).

- **CalculateNumberOfErrors(sudoku)** <br>
Calcule le nombre total d'erreurs dans une grille donnée :
Erreurs dans les lignes et colonnes (valeurs dupliquées).

- **CreateList3x3Blocks()** <br>
Crée une liste de coordonnées représentant les sous-grilles 3x3 du Sudoku.

- **RandomlyFill3x3Blocks(sudoku, listOfBlocks)** <br>
Remplit aléatoirement les cases vides de chaque sous-grille 3x3, en respectant les valeurs déjà présentes.

- **ChooseNewState(sudoku, fixedSudoku, listOfBlocks, sigma)** <br>
Génère une nouvelle configuration potentielle en échangeant des valeurs dans une sous-grille :
Accepte ou rejette cette configuration en fonction de la différence de coût (nombre d'erreurs) et d'une probabilité définie par sigma.

- **CalculateInitialSigma(sudoku, fixedSudoku, listOfBlocks)** <br>
Calcule un facteur de température initial (sigma) en estimant les variations de coût dans les premières étapes.

- **solveSudoku(sudoku)** <br>
Résout un puzzle Sudoku en appliquant l'algorithme de recuit simulé :
Initialisation de la grille avec des valeurs aléatoires.
Optimisation itérative pour réduire les erreurs.
Raffinement progressif via une diminution de la température.
## 3. Utilisation
Fonctionnement général <br>
Définir une grille Sudoku de départ sous forme d'une chaîne de caractères, où 0 représente les cases vides.<br>
Convertir cette grille en un tableau NumPy grâce à la fonction de parsing.<br>
Exécuter la fonction solveSudoku(sudoku) pour obtenir la solution.<br>

## 4. Limitations
Complexité des puzzles : L'algorithme peut être inefficace pour des puzzles extrêmement difficiles avec peu de valeurs initiales.<br>
Temps d'exécution : La méthode probabiliste peut nécessiter un grand nombre d'itérations pour converger, notamment en cas de "blocage".<br>
Validité non garantie : Le résultat final peut nécessiter une validation externe si des erreurs subsistent après optimisation.<br>
## 5. Améliorations futures
Implémenter une meilleure stratégie pour la sélection des cases à modifier (priorité aux cases les plus problématiques).<br>
Ajouter un mécanisme de réinitialisation si l'algorithme reste bloqué trop longtemps dans un état sous-optimal.<br>
Intégrer une méthode de validation automatique pour confirmer l'absence d'erreurs dans la solution finale.<br>
Ce solveur offre une approche robuste et adaptable, bien qu'il repose sur des heuristiques probabilistes, pour résoudre efficacement des puzzles Sudoku de difficulté variable.<br>
## 6. Resultats
Easy : 2s<br>
Medium : 42s<br>
Hard : ~4h20<br>
