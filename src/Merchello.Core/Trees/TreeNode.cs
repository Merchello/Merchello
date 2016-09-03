namespace Merchello.Core.Trees
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a generic tree.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object
    /// </typeparam>
    internal class TreeNode<T>
    {
        /// <summary>
        /// The list of the children.
        /// </summary>
        private readonly LinkedList<TreeNode<T>> _children = new LinkedList<TreeNode<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public TreeNode(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public TreeNode<T> Parent { get; private set; }

        /// <summary>
        /// Gets the value of the node.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IEnumerable<TreeNode<T>> Children
        {
            get
            {
                return this._children;
            }
        }

        /// <summary>
        /// Gets a child at a specific index.
        /// </summary>
        /// <param name="i">
        /// The index of the child to retrieve.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{T}"/>.
        /// </returns>
        public TreeNode<T> this[int i]
        {
            get
            {
                if (this._children.Count() >= i) return null;
                return this._children.ElementAt(i);
            }
        }

        /// <summary>
        /// The add child.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{T}"/>.
        /// </returns>
        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            this._children.AddLast(node);
            return node;
        }

        /// <summary>
        /// Adds children to the tree.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="TreeNode{T}"/>.
        /// </returns>
        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(this.AddChild).ToArray();
        }

        /// <summary>
        /// Removes a child from the tree.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveChild(TreeNode<T> node)
        {
            return this._children.Remove(node);
        }

        /// <summary>
        /// Applies a visitor to each ancestor
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        public void Climb(ITreeNodeVisitor<T> visitor)
        {
            if (!visitor.Completed) visitor.Visit(this);
            if (!visitor.Completed && this.Parent != null) Parent.Climb(visitor);
        }

        /// <summary>
        /// Applies a visitor to each node in a traversal.
        /// </summary>
        /// <param name="visitor">
        /// The action.
        /// </param>
        public void Traverse(ITreeNodeVisitor<T> visitor)
        {
            if (!visitor.Completed) visitor.Visit(this);

            foreach (var child in this._children)
            {
                child.Traverse(visitor);
                if (visitor.Completed) break;
            }
        }

        /// <summary>
        /// Flattens the tree.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> Flatten()
        {
            return new[] { this.Value }.Union(this._children.SelectMany(x => x.Flatten()));
        }
    }
}