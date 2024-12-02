using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Shared;

namespace Sudoku.ProjectTestSolver
//Norvig explication
//La représentation des données : Les grilles de Sudoku sont représentées comme des dictionnaires associant chaque case à ses valeurs.
//Élimination : Réduire les choix possibles pour chaque case en éliminant les valeurs déjà présentes dans sa ligne, colonne et carré.
//Assignation : Si une case ne peut prendre qu'une seule valeur possible, on l'assigne directement.
//Backtracking : Si aucune solution trouvée, essayer différentes options récursivement jusqu’à trouver une solution.
{
    public class NorvigSolver : ISudokuSolver
    {
        private const string Digits = "123456789"; //chiffres possibles pour une case (1:9)
        private static readonly string Rows = "ABCDEFGHI"; //lettres représentant les lignes (A à I)
        private static readonly string Columns = "123456789"; //chiffres représentant les colonnes (1 à 9)
        private static readonly List<string> Boxes = CreateBoxes(); //cases possibles (ex: A1,... I9)
        private static readonly List<List<string>> Units = CreateUnits(); //liste des unités (lignes, colonnes, et carrés 3x3)
        private static readonly Dictionary<string, HashSet<string>> Peers = CreatePeers(); //dictionnaire des "voisins" pour chaque case (cases partageant ligne, colonne ou carré)

        //fonction principale
        public SudokuGrid Solve(SudokuGrid s)
        {
            //Convertir la grille en un dictionnaire représentant les valeurs possibles
            var grid = ParseGrid(s);
            if (grid == null) //Si la grille initiale est invalide
            {
                Console.WriteLine("Invalid grid provided.");
                return s;
            }

            // Résoudre la grille à l'aide de l'algorithme de Norvig
            var result = Search(grid);

            if (result != null) //Si une solution est trouvée
            {
                // Update le sudoku avec la solution
                ApplySolution(s, result);
            }
            else
            {
                Console.WriteLine("No solution found.");
            }

            return s; //Retourner la grille (résolue ou non)
        }

        // Convertir la grille Sudoku en un dictionnaire de valeurs possibles
        private Dictionary<string, string> ParseGrid(SudokuGrid s)
        {
            // Initialiser toutes les cases avec toutes les valeurs possibles
            var possibleValues = Boxes.ToDictionary(box => box, box => Digits);
            //parcourir la grille
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    int value = s.Cells[r, c];
                    if (value != 0)  //si une case contient une valeur
                    {
                        // Tenter d'assigner cette valeur à la case correspondante
                        if (!Assign(possibleValues, BoxName(r, c), value.ToString()))
                        {
                            return null; // Si l'assignation échoue, gille invalide
                        }
                    }
                }
            }

            return possibleValues; //retourner le dictionnaire des valeurs possibles
        }

        // Fonction récursive pour résoudre le Sudoku par backtracking
        private Dictionary<string, string> Search(Dictionary<string, string> grid)
        {
            // Si toutes les cases ont une valeur unique, la grille est résolue
            if (Boxes.All(box => grid[box].Length == 1))
            {
                return grid; //on retourne la solution
            }

            //Sélectionner la case avec le moins de possibilités (> 1)
            string box = Boxes.Where(b => grid[b].Length > 1)
                              .OrderBy(b => grid[b].Length)
                              .First();

            //Tenter chaque valeur possible pour cette case
            foreach (var value in grid[box])
            {
                var copy = new Dictionary<string, string>(grid); //Faire une copie de la grille
                if (Assign(copy, box, value.ToString())) //Essayer d'assigner la valeur
                {
                    var result = Search(copy);  //Appeler récursivement la méthode Search
                    if (result != null) //si une solution est trouvée
                    {
                        return result; //retourner la solution
                    }
                }
            }

            return null; // Aucun résultat, pas de solution
        }

        // Assigner une valeur à une case et propager les contraintes
        private bool Assign(Dictionary<string, string> grid, string box, string value)
        {
            //Supprimer toutes les autres valeurs possibles de cette case
            var otherValues = grid[box].Replace(value, "");
            foreach (var other in otherValues)
            {
                if (!Eliminate(grid, box, other.ToString())) //Propager les contraintes
                {
                    return false;
                }
            }

            return true;
        }

        // Éliminer une valeur possible d'une case et propager les contraintes
        private bool Eliminate(Dictionary<string, string> grid, string box, string value)
        {
            if (!grid[box].Contains(value)) //Si la valeur est déjà éliminée
            {
                return true; 
            }

            // Retirer la valeur de la liste des possibilités pour cette case
            grid[box] = grid[box].Replace(value, "");

            // Si la case n'a plus de possibilité, la grille est invalide
            if (grid[box].Length == 0)
            {
                return false;
            }
            // Si la case n'a qu'une seule valeur possible, éliminer cette valeur de ses voisins
            if (grid[box].Length == 1)
            {
                var soleValue = grid[box];
                foreach (var peer in Peers[box])
                {
                    if (!Eliminate(grid, peer, soleValue))
                    {
                        return false;
                    }
                }
            }

            //Si une unité n'a qu'une case où une valeur peut être placée, l'assigner à cette case
            foreach (var unit in Units.Where(u => u.Contains(box)))
            {
                var places = unit.Where(b => grid[b].Contains(value)).ToList();
                if (places.Count == 0) //Si aucune place possible pour cette valeur, grille invalide
                {
                    return false;
                }

                if (places.Count == 1) //Si une seule place possible, assigner la valeur
                {
                    if (!Assign(grid, places[0], value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Appliquer une solution trouvée à l'objet SudokuGrid
        private void ApplySolution(SudokuGrid s, Dictionary<string, string> solution)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    s.Cells[r, c] = int.Parse(solution[BoxName(r, c)]);
                }
            }
        }

        //Générer le nom d'une case (ex: A1, B2...) à partir des indices de ligne et de colonne
        private static string BoxName(int row, int col)
        {
            return $"{Rows[row]}{Columns[col]}";
        }

        //Générer la liste de toutes les cases (A1, A2, ..., I9)
        private static List<string> CreateBoxes()
        {
            return (from r in Rows
                    from c in Columns
                    select $"{r}{c}").ToList();
        }

        //Générer toutes les unités (lignes, colonnes, carrés 3x3)
        private static List<List<string>> CreateUnits()
        {
            var units = new List<List<string>>();

            // Ligne
            for (int r = 0; r < 9; r++)
            {
                units.Add(Columns.Select(c => $"{Rows[r]}{c}").ToList());
            }

            // Colonne
            for (int c = 0; c < 9; c++)
            {
                units.Add(Rows.Select(r => $"{r}{Columns[c]}").ToList());
            }

            // Carré (3*3)
            var rowGroups = new[] { "ABC", "DEF", "GHI" };
            var colGroups = new[] { "123", "456", "789" };
            foreach (var rg in rowGroups)
            {
                foreach (var cg in colGroups)
                {
                    units.Add((from r in rg
                               from c in cg
                               select $"{r}{c}").ToList());
                }
            }

            return units;
        }

        // Générer les voisins pour chaque case
        private static Dictionary<string, HashSet<string>> CreatePeers()
        {
            var peers = new Dictionary<string, HashSet<string>>();

            foreach (var box in Boxes)
            {
                peers[box] = new HashSet<string>(Units.Where(u => u.Contains(box))
                                                      .SelectMany(u => u)
                                                      .Where(b => b != box));
            }

            return peers;
        }
    }
}