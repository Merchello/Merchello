namespace Merchello.Core.Trees
{
    /// <summary>
    /// Aggregates nodes along a traversal path.
    /// </summary>
    /// <typeparam name="TNode">
    /// The type of the node value
    /// </typeparam>
    internal class NodeAggregatorTreeNodeVisitor<TNode> : TreeNodeVistorBase<TNode>
    {
        /// <inheritdoc/>
        public override void Visit(TreeNode<TNode> item)
        {
            this.Enqueue(item);
        }
    }
}