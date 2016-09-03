namespace Merchello.Core.Trees
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// A visitor to find a matching node.
    /// </summary>
    /// <typeparam name="TNode">
    /// The type of the search value
    /// </typeparam>
    internal class FirstByValueTreeNodeVisitor<TNode> : TreeNodeVistorBase<TNode>
        where TNode : class, IHasKeyId
    {
        /// <summary>
        /// The target value.
        /// </summary>
        private readonly TNode _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstByValueTreeNodeVisitor{TNode}"/> class.
        /// </summary>
        /// <param name="value">
        /// The target value.
        /// </param>
        public FirstByValueTreeNodeVisitor(TNode value)
        {
            Ensure.ParameterNotNull(value, "The search value was null");
            _target = value;
        }

        /// <inheritdoc/>
        public override void Visit(TreeNode<TNode> item)
        {
            if (item.Value.Key == _target.Key)
            {
                this.Enqueue(item);
                this.Completed = true;
            }
        }
    }
}