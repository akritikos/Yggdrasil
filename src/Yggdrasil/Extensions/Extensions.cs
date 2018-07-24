using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yggdrasil.Extensions
{
    using System;
    using Accord.MachineLearning.DecisionTrees;

    public static class Extensions
    {
        public static ComparisonKind FlipOperator(this ComparisonKind o)
        {
            switch (o)
            {
                case ComparisonKind.Equal:
                    return ComparisonKind.NotEqual;
                case ComparisonKind.NotEqual:
                    return ComparisonKind.Equal;
                case ComparisonKind.GreaterThanOrEqual:
                    return ComparisonKind.LessThan;
                case ComparisonKind.GreaterThan:
                    return ComparisonKind.LessThanOrEqual;
                case ComparisonKind.LessThan:
                    return ComparisonKind.GreaterThan;
                case ComparisonKind.LessThanOrEqual:
                    return ComparisonKind.GreaterThan;
                case ComparisonKind.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(o), o, null);
            }
        }

        public static IEnumerable<T> ShuffleLinq<T>(this IEnumerable<T> source, Random rnd = null, int size = 0)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            var tmp = source.ToList();
            rnd = rnd ?? new Random();
            size = size == 0 ? tmp.Count : size;

            return tmp
                .Select(x => new {Number = rnd.Next(), Item = x})
                .OrderBy(x => x.Number)
                .Select(x => x.Item)
                .Take(size);
        }

        public static IEnumerable<T> ShuffleFisherYates<T>(this IEnumerable<T> source, Random rnd = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.ShuffleIterator(rnd);
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rnd)
        {
            var buffer = source.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = rnd.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[j];
            }
        }
    }
}
