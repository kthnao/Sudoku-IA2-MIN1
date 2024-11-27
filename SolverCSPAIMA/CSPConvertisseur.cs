namespace Sudoku.SolverCSPAIMA
{
     
    public class SudokuCSPHelper
    {
        static SudokuCSPHelper()
        {
            GetSudokuBaseCSP();
        }

        // Fonction principale pour construire le CSP à partir d'un masque de Sudoku
        public static CSP GetSudokuCSP(SudokuGrid s)
        {
            var toReturn = GetSudokuBaseCSP();

            // Ajout des contraintes spécifiques au masque
            var mask = new Dictionary<int, int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var cellVal = s.Cells[i][j];
                    if (cellVal != 0)
                    {
                        mask[i * 9 + j] = cellVal;
                    }
                }
            }

            var cellVars = toReturn.getVariables();
            var maskQueue = new Queue<int>(mask.Keys);
            var currentMaskIdx = maskQueue.Dequeue();
            var currentVarName = GetVarName(currentMaskIdx / 9, currentMaskIdx % 9);

            foreach (Variable objVar in cellVars.toArray())
            {
                if (objVar.getName() == currentVarName)
                {
                    var cellValue = mask[currentMaskIdx];
                    toReturn.setDomain(objVar, new Domain(new object[] { cellValue }));
                    if (maskQueue.Count == 0) break;
                    currentMaskIdx = maskQueue.Dequeue();
                    currentVarName = GetVarName(currentMaskIdx / 9, currentMaskIdx % 9);
                }
            }

            return toReturn;
        }

        private static CSP GetSudokuBaseCSP()
        {
            var toReturn = new DynamicCSP();
            var cellPossibleValues = Enumerable.Range(1, 9);
            var cellDomain = new Domain(cellPossibleValues.Cast<object>().ToArray());

            // Variables
            var variables = new Dictionary<int, Dictionary<int, Variable>>();
            for (int rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                var rowVars = new Dictionary<int, Variable>();
                for (int colIndex = 0; colIndex < 9; colIndex++)
                {
                    var varName = GetVarName(rowIndex, colIndex);
                    var cellVariable = new Variable(varName);
                    toReturn.AddNewVariable(cellVariable);
                    toReturn.setDomain(cellVariable, cellDomain);
                    rowVars[colIndex] = cellVariable;
                }
                variables[rowIndex] = rowVars;
            }

            // Contraintes (lignes, colonnes, boîtes)
            var contraints = new List<Constraint>();
            foreach (var objPair in variables)
            {
                var ligneVars = objPair.Value.Values.ToList();
                var ligneContraintes = GetAllDiffConstraints(ligneVars);
                contraints.AddRange(ligneContraintes);
            }

            // Colonnes
            for (int j = 0; j < 9; j++)
            {
                var colVars = variables.Values.Select(x => x[j]).ToList();
                var colContraintes = GetAllDiffConstraints(colVars);
                contraints.AddRange(colContraintes);
            }

            // Boîtes
            for (int b = 0; b < 9; b++)
            {
                var boiteVars = new List<Variable>();
                var iStart = 3 * (b / 3);
                var jStart = 3 * (b % 3);
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        boiteVars.Add(variables[iStart + i][jStart + j]);
                    }
                }
                var boitesContraintes = GetAllDiffConstraints(boiteVars);
                contraints.AddRange(boitesContraintes);
            }

            // Ajouter toutes les contraintes
            foreach (var constraint in contraints)
            {
                toReturn.addConstraint(constraint);
            }

            return (CSP)toReturn.Clone();
        }

        private static string GetVarName(int rowIndex, int colIndex)
        {
            return $"cell{rowIndex}{colIndex}";
        }

        public static IEnumerable<Constraint> GetAllDiffConstraints(IList<Variable> vars)
        {
            var toReturn = new List<Constraint>();
            for (int i = 0; i < vars.Count; i++)
            {
                for (int j = i + 1; j < vars.Count; j++)
                {
                    var diffContraint = new NotEqualConstraint(vars[i], vars[j]);
                    toReturn.Add(diffContraint);
                }
            }

            return toReturn;
        }
    }
}