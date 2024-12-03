namespace Sudoku.Z3Solvers;
using Microsoft.Z3;
using Sudoku.Shared;

public class Z3BitVectSubstituteSolver : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        using (Context ctx = new Context())
        {
            BitVecExpr[,] z3Grid = new BitVecExpr[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    z3Grid[i, j] = ctx.MkBVConst($"x_{i}_{j}", 4);  // 4 bits pour représenter les valeurs 1-9
                }
            }
            Solver z3Solver = ctx.MkSolver();

            // Contraintes de valeur des cellules entre 1 et 9
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    z3Solver.Add(ctx.MkAnd(
                        ctx.MkBVULE(ctx.MkBV(1, 4), z3Grid[i, j]),  // 1 <= z3Grid[i, j]
                        ctx.MkBVULE(z3Grid[i, j], ctx.MkBV(9, 4))  // z3Grid[i, j] <= 9
                    ));
                }
            }

            // Contraintes pour chaque ligne (valeurs distinctes)
            for (int i = 0; i < 9; i++)
            {
                z3Solver.Add(ctx.MkDistinct(new BitVecExpr[]{
                    z3Grid[i,0], z3Grid[i,1], z3Grid[i,2], z3Grid[i,3], z3Grid[i,4], z3Grid[i,5], z3Grid[i,6], z3Grid[i,7], z3Grid[i,8]
                }));
            }

            // Contraintes pour chaque colonne (valeurs distinctes)
            for (int j = 0; j < 9; j++)
            {
                z3Solver.Add(ctx.MkDistinct(new BitVecExpr[]{
                    z3Grid[0,j], z3Grid[1,j], z3Grid[2,j], z3Grid[3,j], z3Grid[4,j], z3Grid[5,j], z3Grid[6,j], z3Grid[7,j], z3Grid[8,j]
                }));
            }

            // Contraintes pour chaque sous-grille 3x3 (valeurs distinctes)
            for (int blocRow = 0; blocRow < 3; blocRow++)
            {
                for (int blocCol = 0; blocCol < 3; blocCol++)
                {
                    BitVecExpr[] bloc = new BitVecExpr[9];
                    int compteur = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            bloc[compteur++] = z3Grid[blocRow * 3 + i, blocCol * 3 + j];
                        }
                    }
                    z3Solver.Add(ctx.MkDistinct(bloc));
                }
            }

            // Contraintes des valeurs initiales
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.Cells[i, j] != 0)
                    {
                        BitVecExpr initialValue = ctx.MkBV(s.Cells[i, j], 4); 
                        BoolExpr constraint = ctx.MkEq(z3Grid[i, j], initialValue);
                        z3Solver.Add(constraint);
                    }
                }
            }

            // Résolution du Sudoku
            if (z3Solver.Check() == Status.SATISFIABLE)
            {
                Model model = z3Solver.Model;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        s.Cells[i, j] = (int)((BitVecNum)model.Eval(z3Grid[i, j])).UInt64;  // Convertir BitVecNum en entier
                    }
                }
            }
            else if (z3Solver.Check() == Status.UNSATISFIABLE)
            {
                Console.WriteLine("Impossible de résoudre le sudoku");
            }
            else
            {
                Console.WriteLine("Impossible de savoir si le sudoku est résolvable");
            }

            return s;
        }
    }
}