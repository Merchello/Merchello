using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Config;
using Merchello.Examine.DataServices;

namespace Merchello.Examine
{
    /// <summary>
    /// Extension methods for IndexSet
    /// </summary>
    public static class IndexSetExtensions
    {
        private static readonly object Locker = new object();

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
                set.IndexParentId
                );
        }

    }
}