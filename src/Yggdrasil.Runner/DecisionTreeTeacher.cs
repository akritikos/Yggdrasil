using System.Collections.Generic;
using System.Linq;
using Accord;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Statistics.Filters;
using YggdrasilDataset.Model.Elements;

namespace Yggdrasil.Runner
{
    public class C45Creator
    {
        private static readonly DoubleRange range = new DoubleRange(-1.0, 10.0);

        private static readonly C45Learning teacher = new C45Learning
        {
            DecisionVariable.Continuous("GE 1", range),
            DecisionVariable.Continuous("GE 2", range),
            DecisionVariable.Continuous("GE 3", range),
            DecisionVariable.Continuous("GE 4", range),
        };

        private double[][] rawInput;
        private string[] rawOutput;

        public C45Creator(IEnumerable<StudentData> data)
        {
            var dataSet = data.ToList();
            var rawInput = new double[dataSet.Count][];
            var rawOutput = new string[dataSet.Count];
            var index = 0;
            foreach (var d in dataSet)
            {
                rawInput[index] = d.Reports.ToArray();
                rawOutput[index] = d.Result.ToString();
                index++;
            }

            var codebook = new Codification("Output", rawOutput);
            var output = codebook.Transform("Output", rawOutput);
            Tree = teacher.Learn(rawInput, output);
        }

        public DecisionTree Tree;
    }
}
