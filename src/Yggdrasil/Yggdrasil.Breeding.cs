using System.Diagnostics;
using Accord.IO;
using Yggdrasil.Extensions;

namespace Yggdrasil
{
    using System;
    using System.Linq;
    using Accord.MachineLearning.DecisionTrees;

    public partial class Yggdrasil
    {
        private void Mutate()
        {
            var mutants = _population
                .Where(c => Rnd.NextDouble() < BreedConfiguration.MutationChance);
            mutants.AsParallel().ForAll(tree =>
            {
                var node = tree.ToArray()[Rnd.Next(0, tree.Count() - 1)];

                if (node.IsLeaf)
                {
                    node.Output = RandomOutcome;
                }
                else
                {
                    var r = RandomDataValue();
                    foreach (var n in node.Branches)
                    {
                        n.Value = r;
                    }
                }
            });
        }

        private void Crossover()
        {
            var parents = _population
                .Where(c => Rnd.NextDouble() < BreedConfiguration.CrossoverChance)
                .ShuffleFisherYates(Rnd).ToList();

            if (parents.Count % 2 != 0)
            {
                parents.Remove(parents.Last());
            }

            for (var i = 0; i < parents.Count - 2; i++)
            {
                var (left, right) = CrossSubtrees(parents[i], parents[i + 1]);
                parents[i] = left;
                parents[i + 1] = right;
            }
        }

        public (DecisionTree Left, DecisionTree Right) CrossSubtrees(DecisionTree left, DecisionTree right)
        {
            var leftIndex = Rnd.Next(0, left.Count() - 1);
            var rightIndex = Rnd.Next(0, right.Count() - 1);

            var leftNode = left.ElementAt(leftIndex).DeepClone();
            var rightNode = right.ElementAt(rightIndex).DeepClone();

            var refLeft = left.ElementAt(leftIndex).Branches;
            var refRight = right.ElementAt(rightIndex).Branches;

            refLeft.Clear();
            refRight.Clear();
            refLeft.AddRange(rightNode.Branches);
            refRight.AddRange(leftNode.Branches);

            var fixedLeft = FixTree(left);
            var fixedRight = FixTree(right);

            return (fixedLeft, fixedRight);
        }

        private DecisionNode RecursiveCleanup(DecisionNode node)
        {
            foreach (var nodeBranch in node.Branches)
            {
                RecursiveCleanup(nodeBranch);
                nodeBranch.Owner = null;
                nodeBranch.Parent = null;
                node.Branches.Clear();
            }

            return node;
        }

        private DecisionTree FixTree(DecisionTree tree)
        {
            foreach (var node in tree.Where(t =>
                t.Branches.Any() &&
                (t.Branches[0].Comparison == ComparisonKind.GreaterThan ||
                 t.Branches[0].Comparison == ComparisonKind.GreaterThanOrEqual)))
            {
                var t = node.Branches[0];
                node.Branches[0] = node.Branches[1];
                node.Branches[1] = node.Branches[0];
            }

            foreach (var node in tree.Where(t => t.Owner != tree))
            {
                node.Owner = tree;
            }

            foreach (var node in tree.Where(t => !t.IsLeaf && t.Output.HasValue))
            {
                node.Output = null;
            }

            foreach (var node in tree.Where(t => t.IsLeaf && !t.Output.HasValue))
            {
                node.Output = RandomOutcome;
            }

            tree.Root.Value = null;

            foreach (var node in tree.Where(n => n.Branches.Any(b => b.Parent != n)))
            {
                foreach (var branch in node.Branches)
                {
                    branch.Parent = node;
                }
            }

            return tree;
        }
    }
}
