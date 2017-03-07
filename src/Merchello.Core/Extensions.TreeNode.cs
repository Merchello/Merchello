namespace Merchello.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Trees;

    /// <summary>
    /// Extension methods for <see cref="TreeNode{T}"/>.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Populates the tree from flattened data.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the object to be represented as a tree node
        /// </typeparam>
        /// <returns>
        /// The <see cref="TreeNode{TNode}"/>.
        /// </returns>
        internal static TreeNode<TNode> Populate<TNode>(this TreeNode<TNode> tree, IEnumerable<TNode> data)
            where TNode : IHasKeyId, IHasParent
        {
            var nodes = data as TNode[] ?? data.ToArray();
            var children = nodes.Where(x => x.ParentKey == tree.Value.Key);

            tree.AddChildren(children.ToArray());
            foreach (var child in tree.Children)
            {
                Populate(child, nodes);
            }

            return tree;
        }

        /// <summary>
        /// Gets the siblings of a node.
        /// </summary>
        /// <param name="tree">
        ///     The tree.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda expression
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the tree node value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        internal static IEnumerable<TreeNode<TNode>> Siblings<TNode>(
            this TreeNode<TNode> tree,
            Expression<Func<TreeNode<TNode>, bool>> predicate = null)
              where TNode : IHasKeyId

        {
            if (tree.Parent == null) return Enumerable.Empty<TreeNode<TNode>>();

            // we need to remove the instance we passed in since we only want the siblings
            var nodes = tree.Parent.Children.Where(x => x.Value.Key != tree.Value.Key).AsQueryable();

            return predicate == null ? nodes : nodes.Where(predicate);
        }

        /// <summary>
        /// Gets the ancestors of a node including itself.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the tree node value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        internal static IEnumerable<TreeNode<TNode>> AncestorsOrSelf<TNode>(
            this TreeNode<TNode> tree,
            Expression<Func<TreeNode<TNode>, bool>> predicate = null) where TNode : IHasKeyId
        {
            if (tree.Parent == null) return Enumerable.Empty<TreeNode<TNode>>();
            var visitor = new NodeAggregatorTreeNodeVisitor<TNode>();
            tree.Climb(visitor);

            var nodes = visitor.Nodes.AsQueryable();

            return predicate == null ? nodes : nodes.Where(predicate);
        }

        /// <summary>
        /// Gets the ancestors of a node including itself.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="predicate">
        /// An optional lambda expression.
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the tree node value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        internal static IEnumerable<TreeNode<TNode>> Ancestors<TNode>(
            this TreeNode<TNode> tree,
            Expression<Func<TreeNode<TNode>, bool>> predicate = null)
            where TNode : IHasKeyId
        {
            if (tree.Parent == null) return Enumerable.Empty<TreeNode<TNode>>();
            var visitor = new NodeAggregatorTreeNodeVisitor<TNode>();
            tree.Climb(visitor);

            // the visitor will pick up the tree itself in the traversal so we have to remove it
            var nodes = visitor.Nodes.Where(x => x.Value.Key != tree.Value.Key).AsQueryable();

            return predicate == null ? nodes : nodes.Where(predicate);
        }

        /// <summary>
        /// Gets the descendants of a node.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the tree node value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        internal static IEnumerable<TreeNode<TNode>> DescendantsOrSelf<TNode>(
            this TreeNode<TNode> tree,
            Expression<Func<TreeNode<TNode>, bool>> predicate = null)
            where TNode : IHasKeyId
        {
            var visitor = new NodeAggregatorTreeNodeVisitor<TNode>();
            tree.Traverse(visitor);

            // the visitor will pick up the tree itself in the traversal so we have to remove it
            var nodes = visitor.Nodes.AsQueryable();

            return predicate == null ? nodes : nodes.Where(predicate);
        }

        /// <summary>
        /// Gets the descendants of a node.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the tree node value
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{TreeNode}"/>.
        /// </returns>
        internal static IEnumerable<TreeNode<TNode>> Descendants<TNode>(
            this TreeNode<TNode> tree,
            Expression<Func<TreeNode<TNode>, bool>> predicate = null)
            where TNode : IHasKeyId
        {
            var visitor = new NodeAggregatorTreeNodeVisitor<TNode>();
            tree.Traverse(visitor);

            // the visitor will pick up the tree itself in the traversal so we have to remove it
            var nodes = visitor.Nodes.Where(x => x.Value.Key != tree.Value.Key).AsQueryable();

            return predicate == null ? nodes : nodes.Where(predicate);
        }

        /// <summary>
        /// Finds the first node with a matching key value.
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="TNode">
        /// The type of the tree node value
        /// </typeparam>
        /// <returns>
        /// The <see cref="TreeNode{TNode}"/>.
        /// </returns>
        internal static TreeNode<TNode> FirstByValue<TNode>(this TreeNode<TNode> tree, TNode value)
            where TNode : class, IHasKeyId
        {
            var visitor = new FirstByValueTreeNodeVisitor<TNode>(value);
            tree.Traverse(visitor);

            return visitor.Nodes.FirstOrDefault();
        }
    }
}
