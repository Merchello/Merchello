namespace Merchello.Examine
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Examine;
    using global::Examine.LuceneEngine.Config;

    /// <summary>
    /// Extension methods for IndexSet
    /// </summary>
    public static class IndexSetExtensions
    {
        /// <summary>
        /// The thread locker.
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// Creates an <see cref="IIndexCriteria"/>.
        /// </summary>
        /// <param name="set">
        /// The set.
        /// </param>
        /// <param name="indexFieldNames">
        /// The index field names.
        /// </param>
        /// <param name="indexFieldPolicies">
        /// The index field policies.
        /// </param>
        /// <returns>
        /// The <see cref="IIndexCriteria"/>.
        /// </returns>
        internal static IIndexCriteria ToIndexCriteria(this IndexSet set, IEnumerable<string> indexFieldNames, IEnumerable<StaticField> indexFieldPolicies)
        {
            if (set.IndexAttributeFields.Count == 0)
            {
                foreach (var fn in indexFieldNames)
                {
                    var field = new IndexField() {Name = fn};
                    var policy = indexFieldPolicies.FirstOrDefault(x => x.Name == fn);
                    if (policy != null)
                    {
                        field.Type = policy.Type;
                        field.EnableSorting = policy.EnableSorting;
                    }
                    set.IndexAttributeFields.Add(field);
                }
            }

            return new IndexCriteria(
                set.IndexAttributeFields.Cast<IIndexField>().ToArray(),
                set.IndexUserFields.Cast<IIndexField>().ToArray(),
                set.IncludeNodeTypes.ToList().Select(x => x.Name).ToArray(),
                set.ExcludeNodeTypes.ToList().Select(x => x.Name).ToArray(),
                set.IndexParentId);
        }

    }
}