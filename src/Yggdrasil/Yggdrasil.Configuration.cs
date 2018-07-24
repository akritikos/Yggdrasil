namespace Yggdrasil
{
    using System;
    using System.Collections.Generic;
    using Accord;
    using Accord.MachineLearning.DecisionTrees;
    using YggdrasilDataset.Model.Elements;

    public partial class Yggdrasil
    {
        private static readonly Random Rnd = new Random();
        private readonly int _numOfComparisons = Enum.GetNames(typeof(ComparisonKind)).Length - 1;
        private readonly StudentData[][] _dataSet;
        private readonly int[] _availableOutcomes;
        private readonly double _min;
        private readonly double _max;
        private readonly IEnumerable<DecisionVariable> _variables;
        private List<DecisionTree> _population = new List<DecisionTree>();

        public BreedParameters BreedConfiguration { get; set; }

        public double Accuracy { get; private set; }

        public TimeSpan TotalBreedingTime { get; private set; }

        public List<(DecisionTree tree, double payoff)> RatedTrees { get; set; }

        private int RandomOutcome =>
            _availableOutcomes[Rnd.Next(0, _availableOutcomes.Length - 1)];

        private static bool IsComparisonValid(DecisionVariableKind var, ComparisonKind c) =>
            var == DecisionVariableKind.Discrete
                ? c == ComparisonKind.Equal || c == ComparisonKind.NotEqual
                : c == ComparisonKind.GreaterThan ||
                  c == ComparisonKind.LessThan;

        private static double GetRandomDouble(double min, double max) =>
            min + (Rnd.NextDouble() * (max - min));

        private double RandomDataValue()
        {
            var m = GetRandomDouble(_min, _max);
            if (m < 0)
            {
                m = Math.Round(m, 0);
            }

            return Math.Round(m, 2);
        }

        private static IEnumerable<DecisionVariable> CreateVariables(double min, double max, int count)
        {
            var variables = new DecisionVariable[count];
            var range = new Range((float) min, (float) max);
            for (var i = 0; i < count; i++)
            {
                variables[i] = new DecisionVariable($"GE {i + 1}", range);
            }

            return variables;
        }

        private ComparisonKind RandomComparison(DecisionVariableKind variable)
        {
            ComparisonKind c;
            do
            {
                c = (ComparisonKind) Rnd.Next(1, _numOfComparisons);
            }
            while (!IsComparisonValid(variable, c));

            return c;
        }
    }
}
