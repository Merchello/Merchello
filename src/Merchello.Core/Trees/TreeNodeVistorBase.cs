namespace Merchello.Core.Trees
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// A base class for visiting nodes in a tree.
    /// </summary>
    /// <typeparam name="TNode">
    /// The type of the node value
    /// </typeparam>
    internal abstract class TreeNodeVistorBase<TNode> : ITreeNodeVisitor<TNode>
    {
        /// <summary>
        /// A queue to store found nodes.
        /// </summary>
        private readonly Queue<TreeNode<TNode>> _nodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeVistorBase{TNode}"/> class.
        /// </summary>
        protected TreeNodeVistorBase()
        {
            _nodes = new Queue<TreeNode<TNode>>();
        }

        /// <summary>
        /// Gets the list of nodes.
        /// </summary>
        /// <remarks>
        /// Used to store matched nodes from the tree traversal
        /// </remarks>
        public IEnumerable<TreeNode<TNode>> Nodes
        {
            get
            {
                return _nodes.ToArray();
            }
        }

        /// <summary>
        /// Gets the values of the matched nodes
        /// </summary>
        public IEnumerable<TNode> Values
        {
            get
            {
                return _nodes.ToArray().Select(x => x.Value);
            }
        }

        /// <inheritdoc/>
        public bool Completed { get; set; }


        /// <inheritdoc/>
        public abstract void Visit(TreeNode<TNode> item);

        /// <summary>
        /// Removes a node from the top of the queue.
        /// </summary>
        /// <returns>
        /// The <see cref="TreeNode{TNode}"/>.
        /// </returns>
        protected TreeNode<TNode> Dequeue()
        {
            return _nodes.Dequeue();
        }

        /// <summary>
        /// Adds a node the the queue.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        protected void Enqueue(TreeNode<TNode> node)
        {
            _nodes.Enqueue(node);
        }
    }
}