namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.DataStructures;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Services;

    /// <summary>
    /// The product collection extensions.
    /// </summary>
    public static class ProductCollectionExtensions
    {
        /// <summary>
        /// Get the parent <see cref="ProductCollection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCollection"/>.
        /// </returns>
        public static IProductCollection Parent(this IProductCollection value)
        {
            return value.ParentKey == null ? 
                       null : 
                       GetByKey(value.ParentKey.Value);
        }

        /// <summary>
        /// The child collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollectionr}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> Children(this IProductCollection value)
        {
            return GetChildren(value);
        }

        /// <summary>
        /// Gets the collection of all products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> GetProducts(this IProductCollection value)
        {
            return value.GetProducts(1, long.MaxValue);
        }

        /// <summary>
        /// Gets the collection of all products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// Number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static IEnumerable<IProductContent> GetProducts(
            this IProductCollection value,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            
            return value.GetProductsPaged(page, itemsPerPage, sortBy, sortDirection).Items;
        }

        /// <summary>
        /// Gets the paged collection of products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// Number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        public static PagedCollection<IProductContent> GetProductsPaged(
            this IProductCollection value,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var merchelloHelper = new MerchelloHelper();
            return value.GetProducts(merchelloHelper, page, itemsPerPage, sortBy, sortDirection);
        }


        /// <summary>
        /// Gets a collection of <see cref="ProductCollection"/> that contain the product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollection}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> Collections(this IProductContent product)
        {
            return product.Collections(MerchelloContext.Current);
        }

        public static IEnumerable<IProductCollection> Ancestors(this IProductCollection collection)
        {
            var tree = collection.GetTreeContaining();
            if (tree == null) return Enumerable.Empty<IProductCollection>();
            throw new NotImplementedException();
        }

        public static IEnumerable<IProductCollection> Descendants(this IProductCollection collection)
        {
            var tree = collection.GetTreeContaining();
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the collection of all products in the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <param name="merchelloHelper">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// Number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductContent}"/>.
        /// </returns>
        internal static PagedCollection<IProductContent> GetProducts(
            this IProductCollection value,
            MerchelloHelper merchelloHelper,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            ProductSortField order;
            switch (sortBy.ToLowerInvariant())
            {
                case "price":
                    order = ProductSortField.Price;
                    break;
                case "sku":
                    order = ProductSortField.Sku;
                    break;
                case "name":
                default:
                    order = ProductSortField.Name;
                    break;
            }

            return
                merchelloHelper.ProductContentQuery()
                    .Page(page)
                    .ConstrainBy(value)
                    .ItemsPerPage(itemsPerPage)
                    .OrderBy(order, sortDirection)
                    .Execute();

            ////return merchelloHelper.Query.Product.TypedProductContentFromCollection(
            ////    value.Key,
            ////    page,
            ////    itemsPerPage,
            ////    sortBy,
            ////    sortDirection);
        }

        /// <summary>
        /// Returns a collection of ProductCollection for a given product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollection}"/>.
        /// </returns>
        internal static IEnumerable<IProductCollection> Collections(this IProductContent product, IMerchelloContext merchelloContext)
        {
            return GetProxyService(merchelloContext).GetCollectionsContainingProduct(product.Key);
        }

        /// <summary>
        /// Loads a collection of all <see cref="IProductCollection"/> into a collection of instantiated trees.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="allCollections">
        /// The all collections.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        internal static TreeNode<IProductCollection> Load(this TreeNode<IProductCollection> tree, IEnumerable<IProductCollection> allCollections)
        {
            var collections = allCollections as IProductCollection[] ?? allCollections.ToArray();
            var children = collections.Where(x => x.ParentKey == tree.Value.Key);
            tree.AddChildren(children.ToArray());
            foreach (var child in tree.Children)
            {
                child.Load(collections);
            }

            return tree;
        }

        internal static TreeNode<IProductCollection> DepthFirstSearch(
            this TreeNode<IProductCollection> tree, 
            IProductCollection collection,
            TreeNode<IProductCollection> start = null)
        {
            TreeNode<IProductCollection> found = null;
            var visited = new HashSet<TreeNode<IProductCollection>>();
            var stack = new Stack<TreeNode<IProductCollection>>();
            if (start == null) start = tree;

            stack.Push(start);

            while (stack.Count != 0 && found == null)
            {
                var current = stack.Pop();
                if (current.Value.Key == collection.Key)
                {
                    found = current;
                }
                else
                {
                    if (!visited.Add(current))
                        continue;

                    var children = current.Children.Where(x => visited.All(y => y.Value.Key != x.Value.Key));

                    // If you don't care about the left-to-right order, remove the Reverse
                    foreach (var child in children.Reverse())
                        stack.Push(child);
                }
            }

            return found;
        }

        /// <summary>
        /// The get tree containing.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        internal static TreeNode<IProductCollection> GetTreeContaining(this IProductCollection value)
        {
            var trees = GetProxyService(MerchelloContext.Current).GetRootLevelCollectionTrees();
            var tree = trees.FirstOrDefault(x => x.Flatten().Any(y => y.Key == value.Key));
            if (tree != null) return tree;

            MultiLogHelper.Warn(typeof(ProductCollectionExtensions), "Failed to find a tree containing the IProductCollection");

            return null;
        }

        /// <summary>
        /// Gets the child collections.
        /// </summary>
        /// <param name="value">
        /// The <see cref="ProductCollection"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollection}"/>.
        /// </returns>
        private static IEnumerable<IProductCollection> GetChildren(this IProductCollection value)
        {
            return GetProxyService(MerchelloContext.Current).GetChildCollections(value.Key);
        } 
        


        /// <summary>
        /// Gets the <see cref="ProductCollection"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCollection"/>.
        /// </returns>
        private static IProductCollection GetByKey(Guid key)
        {
            return GetProxyService(MerchelloContext.Current).GetByKey(key);
        }


        /// <summary>
        /// Gets the <see cref="IProductCollectionService"/>.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollectionService"/>.
        /// </returns>
        private static IProductCollectionService GetProxyService(IMerchelloContext merchelloContext)
        {
            return ProxyEntityServiceResolver.Current.Instance<ProductCollectionService>(new object[] { merchelloContext });
        }
        
    }
}