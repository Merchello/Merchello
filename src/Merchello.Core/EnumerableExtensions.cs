namespace Merchello.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for IEnumerable types
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// An enumerable representing the cartesian product of the sequences
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="sequences">The collections used in the cartesian product</param>
        /// <returns>The cartesian product of the sequences</returns>
        /// <seealso cref="http://stackoverflow.com/questions/3093622/generating-all-possible-combinations"/>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[]
                    {
                        item
                    }));
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        /// <summary>
        /// Finds all possible combinations of the items in the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <typeparam name="T">
        /// The type of value in the collection
        /// </typeparam>
        /// <returns>
        /// A statics in the form of a collection of Tuples representing the level (number of items in the match collection) of the matches and a list of
        /// matches that constitute the matching group.
        /// 
        /// e.g.  For  V1, V2, T3
        /// we expect
        /// 
        /// [
        ///    [1, [V1]], [1, [V2]], [1, [V3]],
        ///    [2, [V1, V2]], [2, [V1, V3]], [2, [V2, V3]]
        ///    [3, [V1, V2, V3]]
        /// ]
        /// 
        /// </returns>
        internal static IEnumerable<Tuple<int, IEnumerable<T>>> AllCombinationsOf<T>(this IEnumerable<T> collection)
        {
            var combos = new List<Tuple<int, IEnumerable<T>>>();

            var collectionArray = collection as T[] ?? collection.ToArray();
            var count = Math.Pow(2, collectionArray.Count());
            for (var i = 1; i <= count - 1; i++)
            {
                var str = Convert.ToString(i, 2).PadLeft(collectionArray.Count(), '0');
                var level = str.ToArray().Where(x => x != '0').Sum(x => int.Parse(x.ToString()));

                var group = new List<T>();

                for (var j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        group.Add(collectionArray[j]);
                    }
                }

                combos.Add(new Tuple<int, IEnumerable<T>>(level, group));
            }

            return combos;
        }
    }
}
