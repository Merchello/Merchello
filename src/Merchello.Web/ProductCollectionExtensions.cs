namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Trees;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;
    using Merchello.Web.Search;

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
        /// <param name="predicate">
        /// An optional lambda expression
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductCollectionr}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> Children(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            return value.AsTreeNode().Children.Values(predicate);
        }

        /// <summary>
        /// Gets the siblings of the <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda expression
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> Siblings(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            return value.AsTreeNode().Siblings().Values(predicate);
        }

        /// <summary>
        /// Gets the siblings of the <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda expression
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> Ancestors(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            return value.AsTreeNode().Ancestors().Values(predicate);
        }

        /// <summary>
        /// Gets the first ancestor matching the expression
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollection"/>.
        /// </returns>
        public static IProductCollection Ancestor(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate)
        {
            return value.AsTreeNode().Ancestors().Values(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Gets the siblings of the <see cref="IProductCollection"/> including itself.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda expression
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> AncestorsOrSelf(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            return value.AsTreeNode().AncestorsOrSelf().Values(predicate);
        }

        /// <summary>
        /// Gets the first descendant matching the expression
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollection"/>.
        /// </returns>
        public static IProductCollection Descendant(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate)
        {
            return value.AsTreeNode().Descendants().Values(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Gets the descendants of the <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda expression
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> Descendants(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            return value.AsTreeNode().Descendants().Values(predicate);
        }

        /// <summary>
        /// Gets the descendants of the <see cref="IProductCollection"/> including itself.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        public static IEnumerable<IProductCollection> DescendantsOrSelf(this IProductCollection value, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            return value.AsTreeNode().DescendantsOrSelf().Values(predicate);
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
            return Query().GetCollectionsContainingProduct(product.Key);
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
            return Query().GetByKey(key);
        }

        /// <summary>
        /// Gets the <see cref="TreeNode{IProductCollection}"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the collection could not be found in the cached collection tree.
        /// </exception>
        private static TreeNode<IProductCollection> AsTreeNode(this IProductCollection value)
        {
            var tree = TreeQuery().GetTreeByValue(value);
            if (tree != null) return tree;

            var nullRef = new NullReferenceException("The product collection was not found in Tree cache");
            MultiLogHelper.Error(typeof(ProductCollectionExtensions), "Tree nod found for collection", nullRef);
            throw nullRef;
        }

        /// <summary>
        /// Gets the values from all nodes in the collection.
        /// </summary>
        /// <param name="nodes">
        /// The collection of nodes.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda express 
        /// </param>
        /// <returns>
        /// The collection <see cref="IProductCollection"/>.
        /// </returns>
        private static IEnumerable<IProductCollection> Values(this IEnumerable<TreeNode<IProductCollection>> nodes, Expression<Func<IProductCollection, bool>> predicate = null)
        {
            var values = nodes.Select(x => x.Value).AsQueryable();

            return predicate != null ? values.Where(predicate) : values;
        }

        /// <summary>
        /// Gets the <see cref="IProductCollectionQuery"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductCollectionQuery"/>.
        /// </returns>
        private static IProductCollectionQuery Query()
        {
            return ProxyQueryManager.Current.Instance<ProductCollectionQuery>();
        }

        /// <summary>
        /// Gets the <see cref="IProductCollectionTreeQuery"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IProductCollectionTreeQuery"/>.
        /// </returns>
        private static IProductCollectionTreeQuery TreeQuery()
        {
            return ProxyQueryManager.Current.Instance<ProductCollectionTreeQuery>();
        }
    }
}