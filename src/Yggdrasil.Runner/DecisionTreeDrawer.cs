using System;
using System.IO;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using YggdrasilDataset.Discriminators;

namespace Yggdrasil
{
    class DecisionTreeDrawer
    {
        private readonly DecisionTree _tree;
        private readonly AdjacencyGraph<DecisionNode, TaggedEdge<DecisionNode, string>> _graph;
        private readonly int paddingValue;

        public DecisionTreeDrawer(DecisionTree tree)
        {
            _tree = tree;
            _graph = new AdjacencyGraph<DecisionNode, TaggedEdge<DecisionNode, string>>();
            BuildGraph(tree.Root);
        }

        public DecisionTreeDrawer(DecisionTree tree, bool isOutcomeZeroIndexed)
            : this(tree)
        {
            paddingValue = isOutcomeZeroIndexed ? +1 : 0;
        }

        public string DrawGraph(string outputFile)
        {
            var graphiz = new GraphvizAlgorithm<DecisionNode, TaggedEdge<DecisionNode, string>>(_graph);
            graphiz.CommonVertexFormat.Shape = GraphvizVertexShape.Box;
            graphiz.FormatVertex += FormatVertex;
            graphiz.FormatEdge += (sender, e) => { e.EdgeFormatter.Label.Value = e.Edge.Tag; };
            graphiz.Generate(new FileDotEngine(), outputFile);
            return graphiz.Generate();
        }

        private void BuildGraph(DecisionNode node)
        {
            foreach (var n in node.Branches)
            {
                if (_graph.Edges.Any(e => e.Source == node && e.Target == n))
                {
                    continue;
                }

                var val = !n.Value.HasValue
                    ? ""
                    : n.Value < 0
                        ? Math.Round(n.Value.Value, 0).ToString()
                        : n.Value.ToString();
                var edge = new TaggedEdge<DecisionNode, string>(
                    node,
                    n,
                    $"{ComparisonExtensions.ToString(n.Comparison)}{n.Value}"
                );
                _graph.AddVerticesAndEdge(edge);
                BuildGraph(n);
            }
        }

        private void FormatVertex(object sender, FormatVertexEventArgs<DecisionNode> e)
        {
            if (e.Vertex.Branches.Any())
            {
                e.VertexFormatter.Label = e.Vertex.Branches.Attribute.Name;
                return;
            }

            e.VertexFormatter.Shape = GraphvizVertexShape.Diamond;
            e.VertexFormatter.Label = e.Vertex.Output.HasValue
                ? ((Outcome) ((int) e.Vertex.Output.Value + paddingValue)).ToString()
                : "Malformed Tree!";
        }

        private sealed class FileDotEngine : IDotEngine
        {
            public string Run(GraphvizImageType image, string dot, string outputFileName)
            {
                if (new FileInfo(outputFileName).Exists)
                    File.Delete(outputFileName);
                using (var sw = new StreamWriter(outputFileName))
                {
                    sw.Write(dot);
                    sw.Close();
                }

                return outputFileName;
            }
        }
    }
}
