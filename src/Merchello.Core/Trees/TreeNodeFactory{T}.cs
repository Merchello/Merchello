namespace Merchello.Core.Trees
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a factory capable of building a Tree of <see cref="TreeNode{T}"/>.
    /// </summary>
    /// <typeparam name="TNode">
    /// Type type of the tree node value
    /// </typeparam>
    internal class TreeNodeFactory<TNode>
        where TNode : IHasKeyId, IHasParent
    {
        /// <summary>
        /// The build trees.
        /// </summary>
        /// <param name="flattened">
        /// The flattened tree data.
        /// </param>
        /// <returns>
        /// The collection of trees found in the flattened data.
        /// </returns>
        public IEnumerable<TreeNode<TNode>> BuildTrees(IEnumerable<TNode> flattened)
        {
            var data = flattened as TNode[] ?? flattened.ToArray();

            return data.Where(x => x.ParentKey == null)
                    .Select(root => new TreeNode<TNode>(root)
                    .Populate(data));
        }
    }
}