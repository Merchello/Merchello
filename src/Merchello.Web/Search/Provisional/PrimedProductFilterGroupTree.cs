namespace Merchello.Web.Search.Provisional
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Merchello.Core;
    using Merchello.Core.Acquired;
    using Merchello.Core.Logging;
    using Merchello.Core.Services;
    using Merchello.Core.Trees;
    using Merchello.Web.Models;

    using Newtonsoft.Json;

    using Umbraco.Core.Cache;
    using Umbraco.Core.IO;

    /// <inheritdoc/>
    internal class PrimedProductFilterGroupTree : IPrimedProductFilterGroupTree
    {
        /// <summary>
        /// The runtime cache key.
        /// </summary>
        private const string CacheKeyBase = "merchello.primedproductfiltergrouptree";

        /// <summary>
        /// The <see cref="IProductService"/>.
        /// </summary>
        private readonly IProductService _productService;

        /// <summary>
        /// A function to get all of the product filter groups.
        /// </summary>
        private readonly Func<IEnumerable<IProductFilterGroup>> _getter;

        /// <summary>
        /// The <see cref="IRuntimeCacheProvider"/>.
        /// </summary>
        private readonly IRuntimeCacheProvider _cache;

        private TreeNode<ProductFilterGroupNode> _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimedProductFilterGroupTree"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="getAll">
        /// A function to get all of the product filter groups.
        /// </param>
        internal PrimedProductFilterGroupTree(IMerchelloContext merchelloContext, Func<IEnumerable<IProductFilterGroup>> getAll)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            Ensure.ParameterNotNull(getAll, "getAll");

            _productService = merchelloContext.Services.ProductService;
            _getter = getAll;
            _cache = merchelloContext.Cache.RuntimeCache; 
        }

        /// <inheritdoc/>
        public TreeNode<ProductFilterGroupNode> GetTreeByValue(IProductFilterGroup value, params Guid[] collectionKeys)
        {
            var tree = GetTree(collectionKeys);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public TreeNode<ProductFilterGroupNode> GetTree(params Guid[] collectionKeys)
        {
            var cacheKey = GetCacheKey(collectionKeys);

            var tree = (TreeNode<ProductFilterGroupNode>)_cache.GetCacheItem(cacheKey);
            if (tree != null) return tree;


            // get all of the filter groups
            var filterGroups = _getter.Invoke().ToArray();

            // create a specific context for each filter group and filter (within the group)
            var contextKeys = GetContextKeys(filterGroups, collectionKeys);

            var tuples = CountKeysThatExistInAllCollections(contextKeys);
            
            tree = BuildTreeNode(filterGroups, tuples, collectionKeys);


            return (TreeNode<ProductFilterGroupNode>)_cache.GetCacheItem(cacheKey, () => tree, TimeSpan.FromHours(6));
        }

        /// <summary>
        /// Dumps the run time cache.
        /// </summary>
        public void ClearCache()
        {
            _cache.ClearCacheByKeySearch(CacheKeyBase);
        }

        private TreeNode<ProductFilterGroupNode> BuildTreeNode(IEnumerable<IProductFilterGroup> groups, IEnumerable<Tuple<IEnumerable<Guid>, int>> tuples, params Guid[] collectionKeys)
        {
            var root = new TreeNode<ProductFilterGroupNode>(new ProductFilterGroupNode { Keys = collectionKeys, Item = null });
            foreach (var g in groups)
            {
                root.AddChild(CreateNodeData(g, tuples, collectionKeys));
            }

            return root;
        }

        private ProductFilterGroupNode CreateNodeData(IProductFilterGroup group, IEnumerable<Tuple<IEnumerable<Guid>, int>> tuples, params Guid[] collectionKeys)
        {
            var contextKey = GetContextKey(group, collectionKeys);

            var primedFilters = new List<IPrimedProductFilter>();

            var results = tuples as Tuple<IEnumerable<Guid>, int>[] ?? tuples.ToArray();

            foreach (var f in group.Filters)
            {
                var filterContextKey = GetContextKey(f, collectionKeys);
                var ppf = new PrimedProductFilter
                              {
                                  Key = f.Key,
                                  Name = f.Name,
                                  ParentKey = f.Key,
                                  ProviderMeta = f.ProviderMeta,
                                  SortOrder = f.SortOrder,
                                  Count = GetCountFromResult(filterContextKey, results),
                                  ExtendedData = f.ExtendedData
                              };
                primedFilters.Add(ppf);
            }

            var node = new ProductFilterGroupNode
                           {
                               Keys = collectionKeys,
                               Item =
                                   new PrimedProductFilterGroup
                                       {
                                           Key = group.Key,
                                           Name = group.Name,
                                           ProviderMeta = group.ProviderMeta,
                                           SortOrder = group.SortOrder,
                                           Count = GetCountFromResult(contextKey, results),
                                           Filters = primedFilters,
                                           ExtendedData = group.ExtendedData
                                       }
                           };

            return node;
        }

        private int GetCountFromResult(Guid[] keys, IEnumerable<Tuple<IEnumerable<Guid>, int>> results)
        {
            var result = results.FirstOrDefault(x => x.Item1.All(keys.Contains));
            return result != null ? result.Item2 : 0;
        }

        private Guid[] GetContextKey(IEntityCollectionProxy entity, params Guid[] collectionKeys)
        {
            if (collectionKeys.Contains(entity.Key)) return collectionKeys;

            var combined = collectionKeys.ToList();
            combined.Add(entity.Key);
            return combined.ToArray();
        }


        private IEnumerable<Tuple<IEnumerable<Guid>, int>> CountKeysThatExistInAllCollections(IEnumerable<Guid[]> contextKeys)
        {
            return ((ProductService)_productService).CountKeysThatExistInAllCollections(contextKeys);
        }

        /// <summary>
        /// Builds a combined list of each possible filter count next given the current context (already filtered keys).
        /// </summary>
        /// <param name="filterGroups">
        /// The filter groups.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The collection of possible filtered keys.
        /// </returns>
        private static IEnumerable<Guid[]> GetContextKeys(IEnumerable<IProductFilterGroup> filterGroups, params Guid[] collectionKeys)
        {
            var contextKeys = new List<Guid[]>();

            var cks = !collectionKeys.Any() ? new Guid[] { } : collectionKeys;

            var groups = filterGroups as IProductFilterGroup[] ?? filterGroups.ToArray();

            try
            {
                if (!groups.Any()) return new[] { cks };

                // we need individual sets of filter group keys, eached combined with the collection keys
                // to create the context for the filter groups
                var groupKeys = groups.Select(x => x.Key).ToArray();

                // then we need the individual filter keys, again combined with the collection keys
                // to create the context for the individual filters
                var filterKeys = new List<Guid>();
                foreach (var fg in groups)
                {
                    filterKeys.AddRange(fg.Filters.Select(f => f.Key));
                }


                // individual filter groups
                foreach (var fgk in groupKeys)
                {
                    var groupContext = cks.Select(x => x).ToList();
                    groupContext.Add(fgk);

                    contextKeys.Add(groupContext.ToArray());
                }

                // individual filters where keys are not already part of the collection keys (that would be a duplicate query)
                foreach (var fk in filterKeys.Where(x => !cks.Contains(x)))
                {
                    var filterContext = cks.Select(x => x).ToList();
                    filterContext.Add(fk);

                    contextKeys.Add(filterContext.ToArray());
                }
            }
            catch (Exception ex)
            {
                MultiLogHelper.WarnWithException<PrimedProductFilterGroupTree>("Failed to query for all filter groups", ex);
            }

            return contextKeys;
        }

        /// <summary>
        /// Creates a cache key.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The cache key.
        /// </returns>
        private static string GetCacheKey(IEnumerable<Guid> keys)
        {
            var suffix = keys.Select(x => x.ToString()).OrderBy(x => x);
            return string.Format("{0}.{1}", CacheKeyBase, string.Join(string.Empty, suffix));
        }
        
    }
}