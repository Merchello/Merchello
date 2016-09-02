namespace Merchello.Core.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Represents a generic tree.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object
    /// </typeparam>
    public class TreeNode<T>
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
                return _children;
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
            _children.AddLast(node);
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
            return values.Select(AddChild).ToArray();
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
            return _children.Remove(node);
        }

        /// <summary>
        /// Performs an action on each node in the tree traversal.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
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
                if (_children.Count() >= i) return null;
                return _children.ElementAt(i);
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
            return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
        }
    }
}