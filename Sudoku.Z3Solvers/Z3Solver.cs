namespace Sudoku.Z3Solvers;
using Microsoft.Z3;
using Sudoku.Shared;

public class Z3Solver : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        using (Context ctx = new Context())
        {
            IntExpr[,] z3Grid = new IntExpr[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    z3Grid[i, j] = ctx.MkIntConst($"x_{i}_{j}");
                }
            }
            Solver z3Solver = ctx.MkSolver();
            
            //contrainte chiffres du tableau entre 1 et 9
            
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    z3Solver.Add(ctx.MkAnd(ctx.MkLe(ctx.MkInt(1), z3Grid[i, j] ), ctx.MkLe(z3Grid[i, j], ctx.MkInt(9))));
                }
            }
            
            //contraintes chaque chiffre differents sur une ligne 
            
            for (int i = 0; i < 9; i++)
            {
                z3Solver.Add(ctx.MkDistinct(new IntExpr[]{
                        z3Grid[i,0],z3Grid[i,1],z3Grid[i,2],z3Grid[i,3],z3Grid[i,4],z3Grid[i,5],z3Grid[i,6],z3Grid[i,7],z3Grid[i,8] 
                    }));
            }
            
            //contraintes chaque chiffre different sur une colonne 
            
            for (int j = 0; j < 9; j++)
            {
                z3Solver.Add(ctx.MkDistinct(new IntExpr[]{
                        z3Grid[0,j],z3Grid[1,j],z3Grid[2,j],z3Grid[3,j],z3Grid[4,j],z3Grid[5,j],z3Grid[6,j],z3Grid[7,j],z3Grid[8,j] 
                    }));
            }
            
            //contrainteq sous-bloc 3x3 
            
            for (int blocRow = 0; blocRow < 3; blocRow++)  //itère dans les différents bloc 3x3
            {
                for (int blocCol = 0; blocCol < 3; blocCol++) //pareil
                { 
                    IntExpr[] bloc = new IntExpr[9]; //données d'un bloc 3x3
                    int compteur = 0;
                    for (int i = 0; i < 3; i++) {           //parcourt le bloc 3x3 
                        for (int j = 0; j < 3; j++) {
                            bloc[compteur++] = z3Grid[blocRow * 3 + i, blocCol * 3 + j]; 
                        } 
                    }
                    z3Solver.Add(ctx.MkDistinct(bloc)); //ajoutes une contrainte pour chaque sous-bloc
                }
            }
            
            //contraintes pour fixer les éléments initiaux de la grille

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.Cells[i, j] != 0)
                    {
                        z3Solver.Add(ctx.MkEq(z3Grid[i, j],ctx.MkInt(s.Cells[i, j])));
                    }
                }
                
            }
            
            //resolution de la grille 

            if (z3Solver.Check() == Status.SATISFIABLE)
            {
                Model model = z3Solver.Model;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        s.Cells[i, j] = ((IntNum)model.Eval(z3Grid[i, j])).Int; ;
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
        }
        return s;
    }
}