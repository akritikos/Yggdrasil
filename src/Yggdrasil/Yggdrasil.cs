using Yggdrasil.Extensions;

namespace Yggdrasil
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Accord.IO;
	using Accord.MachineLearning.DecisionTrees;
	using YggdrasilDataset.Model.Elements;

    public partial class Yggdrasil
    {
        public Yggdrasil(
            BreedParameters parameters,
            IEnumerable<StudentData> set,
            IEnumerable<DecisionTree> startingPopulation = null)
            : this()
        {
            BreedConfiguration = parameters;
            var dataset = set.ShuffleFisherYates(Rnd).ToList();
            var recordsPerFold = (int) Math.Ceiling(dataset.Count / (double) BreedConfiguration.FoldToParts);
            _dataSet = new StudentData[BreedConfiguration.FoldToParts][];

            _availableOutcomes = dataset.GroupBy(x => x.Result).Select(x => (int) x.Key).ToArray();
            _min = dataset.SelectMany(x => x.Reports).Min();
            _max = dataset.SelectMany(x => x.Reports).Max();

            _variables = CreateVariables(_min, _max, set.First().Reports.Count);

            for (var i = 0; i < parameters.FoldToParts; i++)
            {
                _dataSet[i] = dataset.Skip(i * recordsPerFold).Take(recordsPerFold).ToArray();
            }

            _population = startingPopulation?.ToList() ?? new List<DecisionTree>();
            FillPopulation();
        }

        private Yggdrasil()
        {
        }

        public void Breed()
        {
            var span = new TimeSpan(0);
            var elite = (int) Math.Round(BreedConfiguration.ElitismPercent * BreedConfiguration.StartingPopulation);
            for (var fold = 0; fold < BreedConfiguration.FoldToParts - 1; fold++)
            {
                var set = _dataSet[fold];
                for (var generation = 0; generation < BreedConfiguration.GenerationCap; generation++)
                {
                    var watch = Stopwatch.StartNew();
                    Console.Write("\r".PadLeft(Console.WindowWidth - Console.CursorLeft - 1));
                    Console.Write($"Fold: {fold + 1:D3}\t Generation: {generation + 1:D3}");
                    var rated = new List<(DecisionTree tree, double payoff)> {Capacity = _population.Count};
                    _population.AsParallel().ForAll(x => { rated.Add((x, TreeFitness(x, set))); });

                    rated.Sort((x, y) => y.payoff.CompareTo(x.payoff));
                    RatedTrees = rated;
                    var elites = rated.Take(elite).Select(x => x.tree);
                    var chosen = Roulette(rated.Skip(elite));

                    _population = new List<DecisionTree>();
                    _population.AddRange(elites);
                    _population.AddRange(chosen);
                    Crossover();
                    Mutate();
                    watch.Stop();
#if DEBUG
                    Debug.WriteLine(_population.Select(x => x.Count()).Max());
#endif
                    span += watch.Elapsed;
                }
            }

            Accuracy = Evaluate();
            TotalBreedingTime = span;
        }

        private void FillPopulation()
        {
            for (var i = _population.Count; i < BreedConfiguration.StartingPopulation; i++)
            {
                _population.Add(GenerateTree());
            }
        }

        private DecisionTree GenerateTree()
        {
            var tree = new DecisionTree(_variables.ToList(), _availableOutcomes.Length);
            var comparer = RandomComparison(DecisionVariableKind.Continuous);
            var val = RandomDataValue();
            tree.Root = new DecisionNode(tree);
            var col = new DecisionBranchNodeCollection(tree.Root)
            {
                new DecisionNode(tree)
                {
                    Comparison = comparer,
                    Value = val,
                    Parent = tree.Root,
                    Output = RandomOutcome
                },
                new DecisionNode(tree)
                {
                    Comparison = comparer.FlipOperator(),
                    Value = val,
                    Parent = tree.Root,
                    Output = RandomOutcome
                }
            };
            col.AttributeIndex = Rnd.Next(0, _variables.Count());
            tree.Root.Branches = col;
            return tree;
        }
    }
}
