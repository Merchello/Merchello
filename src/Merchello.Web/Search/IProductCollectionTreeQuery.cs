namespace Merchello.Web.Search
{
    using System.Collections.Generic;

    using Merchello.Core.Trees;
    using Merchello.Web.Models;

    /// <summary>
    /// Defines a ProductCollectionTreeProvider.
    /// </summary>
    internal interface IProductCollectionTreeQuery
    {
        /// <summary>
        /// Gets a first tree (or first node within a tree if not the root) that matches the value of the node.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        TreeNode<IProductCollection> GetTreeByValue(IProductCollection value);

        /// <summary>
        /// Gets a tree that contains a node for the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        TreeNode<IProductCollection> GetTreeContaining(IProductCollection collection);

        /// <summary>
        /// Gets a tree with root starting at the current collection. 
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{IProductCollection}"/>.
        /// </returns>
        TreeNode<IProductCollection> GetTreeWithRoot(IProductCollection collection);

        /// <summary>
        /// Gets all of the root level trees.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProductCollection}"/>.
        /// </returns>
        IEnumerable<TreeNode<IProductCollection>> GetRootTrees();
    }
}