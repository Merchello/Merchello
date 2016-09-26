namespace Merchello.Web.Search.Provisional
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Merchello.Core.Trees;
    using Merchello.Web.Models;

    /// <summary>
    /// Represents a ProductFilterGroup.
    /// </summary>
    /// REFACTOR - change the name of this to something more meaningful
    internal interface IPrimedProductFilterGroupTree
    {
        /// <summary>
        /// Gets the tree node value for a specific IProductFilterGroup within a filter context
        /// </summary>
        /// <param name="value">The <see cref="IProductFilterGroup"/></param>
        /// <param name="collectionKeys">The collection keys which define the context</param>
        /// <returns>The <see cref="ProductFilterGroupNode"/></returns>
        /// <remarks>
        /// With no key parameters, the context is assumed to be with no constraining collections.
        /// </remarks>
        TreeNode<ProductFilterGroupNode> GetTreeByValue(IProductFilterGroup value, params Guid[] collectionKeys);

        /// <summary>
        /// Gets the tree node value for the entire filtering context.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys which define the context.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{ProductFilterGroupNode}"/>
        /// </returns>
        /// <remarks>
        /// With no key parameters, the context is assumed to be with no constraining collections.
        /// </remarks>
        TreeNode<ProductFilterGroupNode> GetTree(params Guid[] collectionKeys);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void ClearCache();
    }
}