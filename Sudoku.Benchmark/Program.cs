using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
//using Humanizer;
using Sudoku.Shared;

namespace Sudoku.Benchmark
{
	class Program
    {

#if DEBUG
		public static bool IsDebug = true;
#else
        public static bool IsDebug = false;
#endif

	    static IConfiguration Configuration;

		static void Main(string[] args)
        {

            Console.WriteLine("Benchmarking GrilleSudoku Solvers");


            // Configuration Builder
            var builder = new ConfigurationBuilder()
	            .SetBasePath(Directory.GetCurrentDirectory())
	            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            PythonConfiguration pythonConfig = null;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
				pythonConfig = Configuration.GetSection("PythonConfig:OSX").Get<PythonConfiguration>();
	            
            }
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
			{
				pythonConfig = Configuration.GetSection("PythonConfig:Linux").Get<PythonConfiguration>();
                //LinuxInstaller.InstallPath = "/root/.pyenv/versions/3.10.5";
                //LinuxInstaller.PythonDirectoryName = "/root/.pyenv/versions/3.10.5/bin";
                //LinuxInstaller.LibFileName = "/root/.pyenv/versions/3.10.5/lib/libpython3.10.so";
			}

			if (pythonConfig != null)
			{
				Console.WriteLine("Customizing MacOs/Linux Python Install from appsettings.json file");
				if (!string.IsNullOrEmpty(pythonConfig.InstallPath))
				{
					MacInstaller.InstallPath = pythonConfig.InstallPath;
				}
				if (!string.IsNullOrEmpty(pythonConfig.PythonDirectoryName))
				{
					MacInstaller.PythonDirectoryName = pythonConfig.PythonDirectoryName;
				}
				if (!string.IsNullOrEmpty(pythonConfig.LibFileName))
				{
					MacInstaller.LibFileName = pythonConfig.LibFileName;
				}
            }

            while (true)
            {
                if (IsDebug)
                {
                    if (RunMenu())
                    {
                       break;
                    }

                }
                else
                {
                    try
                    {
                        if (RunMenu())
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }


        private static bool RunMenu()
        {

            Console.WriteLine("Select Mode: \n1-Single Solver Test, \n2-Benchmarks, \n3-Exit program");
            var strMode = Console.ReadLine();
            int.TryParse(strMode, out var intMode);
            //Console.SetBufferSize(130, short.MaxValue - 100);
            switch (intMode)
            {
                case 1:
                    SingleSolverTest();
                    break;
                case 2:
                    Benchmark();
                    break;
                default:
                    return true;
            }

            return false;

        }


        private static void Benchmark()
        {
            Console.WriteLine("Select Benchmark Type: \n1-Quick Benchmark (Easy, 2 Sudokus, 10s max per sudoku, Single invocation), \n2-Quick Benchmark (Medium, 10 Sudokus, 20s max per sudoku, Single invocation), \n3-Quick Benchmark (Hard, 10 Sudokus, 30s max per sudoku, Single invocation), \n4-Complete Benchmark (All difficulties, 1 mn max per sudoku, several invocations), \n5-Return");
            var strMode = Console.ReadLine();
            int.TryParse(strMode, out var intMode);
            //Console.SetBufferSize(130, short.MaxValue - 100);
            switch (intMode)
            {
                case 1:
                    var tempEasy = new QuickBenchmarkSolversEasy();

					//               if (IsDebug)
					//               {
					//	BenchmarkRunner.Run<QuickBenchmarkSolversEasy>(new DebugInProcessConfig());
					//}
					//               else
					//               {
					//	BenchmarkRunner.Run<QuickBenchmarkSolversEasy>();
					//}
					BenchmarkRunner.Run<QuickBenchmarkSolversEasy>();
					break;
                case 2:
                    //Init solvers
                    var tempMedium = new QuickBenchmarkSolversMedium();
                    //BenchmarkRunner.Run<QuickBenchmarkSolvers>(new DebugInProcessConfig());
                    BenchmarkRunner.Run<QuickBenchmarkSolversMedium>();
                    break;
                case 3:
                    //Init solvers
                    var tempHard = new QuickBenchmarkSolversHard();
                    //BenchmarkRunner.Run<QuickBenchmarkSolvers>(new DebugInProcessConfig());
                    BenchmarkRunner.Run<QuickBenchmarkSolversHard>();
                    break;
                case 4:
                    //Init solvers
                    var temp2 = new CompleteBenchmarkSolvers();
                    BenchmarkRunner.Run<CompleteBenchmarkSolvers>();
                    break;
                default:
                    break;
            }

        }

private static void SingleSolverTest()
{
    var solvers = Shared.SudokuGrid.GetSolvers();
    Console.WriteLine("Select difficulty: 1-Easy, 2-Medium, 3-Hard");
    var strDiff = Console.ReadLine();
    int.TryParse(strDiff, out var intDiff);
    SudokuDifficulty difficulty = SudokuDifficulty.Hard;
    switch (intDiff)
    {
        case 1:
            difficulty = SudokuDifficulty.Easy;
            break;
        case 2:
            difficulty = SudokuDifficulty.Medium;
            break;
        case 3:
            difficulty = SudokuDifficulty.Hard;
            break;
        default:
            break;
    }

    var sudokus = SudokuHelper.GetSudokus(difficulty);

    Console.WriteLine($"Choose 10 puzzle indices between 1 and {sudokus.Count}, separated by spaces (e.g., '1 5 10 15 ...')");
    var strIdx = Console.ReadLine();
    var indices = strIdx.Split(' ').Select(x => int.Parse(x.Trim()) - 1).Take(10).ToList();

    Console.WriteLine("Choose a solver:");
    var solverList = solvers.ToList();
    for (int i = 0; i < solvers.Count(); i++)
    {
        Console.WriteLine($"{(i + 1).ToString(CultureInfo.InvariantCulture)} - {solverList[i].Key}");
    }
    var strSolver = Console.ReadLine();
    int.TryParse(strSolver, out var intSolver);
    var solver = solverList[intSolver - 1].Value.Value;

    // Liste pour stocker les moyennes des temps pour chaque Sudoku
    List<double> averageTimesList = new List<double>();

    // Liste pour stocker tous les temps d'exécution pour la médiane globale
    List<double> allExecutionTimes = new List<double>();

    // Liste pour stocker les temps minimum et maximum pour chaque Sudoku
    List<double> minTimesList = new List<double>();
    List<double> maxTimesList = new List<double>();

    // Résoudre 5 fois chaque Sudoku sélectionné (ignorer la première exécution)
    foreach (var idx in indices)
    {
        var targetSudoku = sudokus[idx];
        Console.WriteLine($"\n--- Résolution du Sudoku index {idx + 1} ---");
        Console.WriteLine("Puzzle original:");
        Console.WriteLine(targetSudoku.ToString());

        List<double> executionTimes = new List<double>();

        double minTime = double.MaxValue;
        double maxTime = double.MinValue;

        for (int i = 0; i < 6; i++) // Exécuter 6 fois mais ignorer la première
        {
            Console.WriteLine($"\n--- Exécution {i + 1}/6 ---");

            var cloneSudoku = targetSudoku.CloneSudoku();
            var sw = Stopwatch.StartNew();

            cloneSudoku = solver.Solve(cloneSudoku);

            sw.Stop();
            var elapsedMilliseconds = sw.Elapsed.TotalMilliseconds;

            if (i > 0) // Ignorer la première exécution
            {
                executionTimes.Add(elapsedMilliseconds);
                allExecutionTimes.Add(elapsedMilliseconds);

                // Calculer le temps minimum et maximum
                if (elapsedMilliseconds < minTime)
                    minTime = elapsedMilliseconds;

                if (elapsedMilliseconds > maxTime)
                    maxTime = elapsedMilliseconds;
            }

            if (!cloneSudoku.IsValid(targetSudoku))
            {
                Console.WriteLine($"Invalid Solution: Solution has {cloneSudoku.NbErrors(targetSudoku)} errors");
                Console.WriteLine("Invalid solution:");
            }
            else
            {
                Console.WriteLine("Valid solution:");
            }

            Console.WriteLine(cloneSudoku.ToString());
            Console.WriteLine($"Time to solution: {elapsedMilliseconds} ms");
        }

        // Calcul de la moyenne et de la médiane pour ce Sudoku
        double averageTime = executionTimes.Average();
        double medianTime = CalculateMedian(executionTimes);
        averageTimesList.Add(averageTime);
        minTimesList.Add(minTime);
        maxTimesList.Add(maxTime);

        Console.WriteLine($"\n--- Statistiques pour le Sudoku index {idx + 1} ---");
        Console.WriteLine($"Temps moyen (sans la première exécution) : {averageTime:F2} ms");
        Console.WriteLine($"Temps médian (sans la première exécution) : {medianTime:F2} ms");
        Console.WriteLine($"Temps minimum : {minTime:F2} ms");
        Console.WriteLine($"Temps maximum : {maxTime:F2} ms");
    }

    // Calcul de la moyenne des moyennes
    double overallAverageTime = averageTimesList.Average();

    // Calcul de la médiane globale
    double globalMedianTime = CalculateMedian(allExecutionTimes);

    // Calcul du temps minimum et maximum global
    double globalMinTime = minTimesList.Min();
    double globalMaxTime = maxTimesList.Max();

    Console.WriteLine($"\n--- Statistiques Globales ---");
    Console.WriteLine($"Moyenne des moyennes : {overallAverageTime:F2} ms");
    Console.WriteLine($"Médiane globale : {globalMedianTime:F2} ms");
    Console.WriteLine($"Temps minimum global : {globalMinTime:F2} ms");
    Console.WriteLine($"Temps maximum global : {globalMaxTime:F2} ms");
}

// Méthode pour calculer la médiane
private static double CalculateMedian(List<double> values)
{
    values.Sort();
    int count = values.Count;
    if (count % 2 == 0)
    {
        // Si le nombre d'éléments est pair, prendre la moyenne des deux éléments centraux
        return (values[count / 2 - 1] + values[count / 2]) / 2.0;
    }
    else
    {
        // Si le nombre d'éléments est impair, prendre l'élément central
        return values[count / 2];
    }
}








    }
}