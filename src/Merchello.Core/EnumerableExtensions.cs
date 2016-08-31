namespace Merchello.Core
{
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
        /// <typeparam name="T"></typeparam>
        /// <param name="sequences"></param>
        /// <returns>An enumerable representing the cartesian product of the sequences</returns>
        /// <remarks>
        /// http://stackoverflow.com/questions/3093622/generating-all-possible-combinations
        /// </remarks>
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
    }
}
