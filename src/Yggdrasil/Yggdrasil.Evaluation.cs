namespace Yggdrasil
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Accord.MachineLearning.DecisionTrees;
	using YggdrasilDataset.Model.Elements;

    public partial class Yggdrasil
    {
        public delegate double TreeFit(DecisionTree tree, IEnumerable<StudentData> dataset, bool isCustom = false);

        public double Evaluate()
        {
            var set = _dataSet[BreedConfiguration.FoldToParts - 1];
            var correct = (
                    from t in set
                    let prediction = Predict(t.Reports.ToArray())
                    where prediction == (int) t.Result
                    select t)
                .Count();

            return Math.Round(correct / (double) set.Length, 4);
        }

        public int Predict(IEnumerable<double> data)
        {
            var results = new Dictionary<int, int>();
            var inv = 0;
            foreach (var outcome in _availableOutcomes)
            {
                results.Add(outcome, 0);
            }

            foreach (var tree in _population)
            {
                var decision = tree.Decide(data.ToArray());
                if (!results.ContainsKey(decision))
                {
                    inv++;
                }
                else
                {
                    results[decision]++;
                }
            }

            return results.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        public double TreeFitness(DecisionTree tree, IEnumerable<StudentData> dataset)
        {
            var correctClassifications = dataset.AsParallel()
                .Count(data => tree.Decide(data.Reports.ToArray()) == (int)data.Result);
            var treeSize = tree.Count();

            return (double) (correctClassifications * correctClassifications * BreedConfiguration.LargeTreePenalty) /
                   ((treeSize * treeSize) + BreedConfiguration.LargeTreePenalty);
        }

        private static IEnumerable<DecisionTree> Roulette(IEnumerable<(DecisionTree tree, double payoff)> rated)
        {
            var set = rated.ToList();
            var cumulativeSum = new double[set.Count];
            double sum = 0;
            for (var i = 0; i < set.Count; i++)
            {
                cumulativeSum[i] = set[i].payoff + sum;
                sum = cumulativeSum[i];
            }

            var distribution = new double[set.Count];
            for (var i = 0; i < set.Count; i++)
            {
                distribution[i] = GetRandomDouble(0, cumulativeSum[i]);
            }

            var keep = new List<DecisionTree>();
            for (var i = 0; i < set.Count; i++)
            {
                for (int j = 0; j < distribution.Length; j++)
                {
                    if (distribution[i] < cumulativeSum[j])
                    {
                        keep.Add(set[i].tree);
                        break;
                    }
                }
            }

            return keep;
        }
    }
}
