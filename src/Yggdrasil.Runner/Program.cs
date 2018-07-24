using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using YggdrasilDataset.Model;

namespace Yggdrasil.Runner
{
    using System;

    public class Program
    {
        public static Random Rnd = new Random();

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var data = new PliData(@"C:\Software\Users\akritikos\Desktop\Thesis\Kritikos_2017-2018\PLI_DATA.csv");
            var set = data.Modules
                .Where(x => x.Title == "pli_10")
                .SelectMany(x => x.Data
                    .Where(y => y.Year.Year == 2010)
                    .SelectMany(y => y.StudentData
                        .Where(z => z.Reports.Count == 4)));

            var breedConfig = new BreedParameters
            {
                FoldToParts = 5,
                MutationChance = 0.10,
                CrossoverChance = 1,
                ElitismPercent = 0.30,
                LargeTreePenalty = 5000,
                StartingPopulation = 500,
                GenerationCap = 10
            };
            var recordsPerFold = (int) Math.Ceiling(set.Count() / (double) breedConfig.StartingPopulation);
            var trainingSet = set.Take((breedConfig.FoldToParts - 1) * recordsPerFold).ToList();
            var evalSet = set.Skip((breedConfig.FoldToParts - 1) * recordsPerFold);

            var c45Representative = new C45Creator(trainingSet).Tree;
            var c45Cheater = new C45Creator(set).Tree;
            var c45cheat = 0;
            var c45Complete = 0;
            foreach (var eval in trainingSet)
            {
                var cheatOutcome = c45Cheater.Decide(eval.Reports.ToArray()) + 1;
                var cheat = cheatOutcome == (int) eval.Result;
                var honestOutcome = c45Representative.Decide(eval.Reports.ToArray()) + 1;
                var correct = honestOutcome == (int) eval.Result;
                c45Complete = correct ? +1 : c45Complete;
                c45cheat = cheat ? +1 : c45cheat;
            }

            var c45AccuracyComplete = Math.Round(10 * c45Complete / (double) trainingSet.Count, 4);
            var c45AccuracyPartial = Math.Round(10 * c45cheat / (double) set.Count(), 4);
            //Console.WriteLine($"C45 accuracy with complete report data: {c45AccuracyComplete:P}");
            //Console.WriteLine($"C45 accuracy with partial report data: {c45AccuracyPartial:P}");
            new DecisionTreeDrawer(c45Representative,true).DrawGraph(
                @"D:\Users\akritikos\Downloads\Arch\dot\c45Representative.dot");
            new DecisionTreeDrawer(c45Cheater,true).DrawGraph(
                @"D:\Users\akritikos\Downloads\Arch\dot\c45Cheater.dot");
            Console.WriteLine("Breeding new forest:");
            var forest = new Yggdrasil(breedConfig, set);
            forest.Breed();
            var trees = new List<(DecisionTree tree, double payoff)>();
            trees = forest.RatedTrees;
            trees.Sort((x, y) => y.payoff.CompareTo(x.payoff));
            var i = 0;
            foreach (var t in trees.Select(t => t.tree).Take(5))
            {
                var drawer = new DecisionTreeDrawer(t);
                drawer.DrawGraph($@"D:\Users\akritikos\Downloads\Arch\dot\payoff-{i}-legacy.dot");
                i++;
            }

            i = 0;
            foreach (var t in trees.Select(t => t.tree).OrderByDescending(t => t.Count()).Take(5))
            {
                var drawer = new DecisionTreeDrawer(t);
                drawer.DrawGraph($@"D:\Users\akritikos\Downloads\Arch\dot\count-{i}-legacy.dot");
                i++;
            }

            var biasedTrees = new List<DecisionTree>();
            for (var j = 0; j < breedConfig.FoldToParts - 2; j++)
            {
                var creator = new C45Creator(set.Skip(recordsPerFold * j).Take(recordsPerFold));
                biasedTrees.Add(creator.Tree);
            }
            Console.WriteLine("\nBreeding new forest with C4.5 trees as initial population");
            var biasedForest = new Yggdrasil(breedConfig, set, biasedTrees);
            biasedForest.Breed();
            trees = biasedForest.RatedTrees;
            trees.Sort((x, y) => y.payoff.CompareTo(x.payoff));

            i = 0;
            foreach (var t in trees.Select(t => t.tree).Take(5))
            {
                var drawer = new DecisionTreeDrawer(t);
                drawer.DrawGraph($@"D:\Users\akritikos\Downloads\Arch\dot\payoff-{i}-biased.dot");
                i++;
            }

            i = 0;
            foreach (var t in trees.Select(t => t.tree).OrderByDescending(t => t.Count()).Take(5))
            {
                var drawer = new DecisionTreeDrawer(t);
                drawer.DrawGraph($@"D:\Users\akritikos\Downloads\Arch\dot\count-{i}-biased.dot");
                i++;
            }

            Console.WriteLine(
                $"\nBreeding took: {forest.TotalBreedingTime.ToString()}\tTotal Accuracy: {forest.Accuracy:P}");
            Console.WriteLine(
                $"\nBreeding took: {biasedForest.TotalBreedingTime.ToString()}\tTotal Accuracy: {biasedForest.Accuracy:P}");
            Console.ReadLine();
        }
    }
}
