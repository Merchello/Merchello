namespace Merchello.Core.Trees
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A tree traversal visitor that applies a lambda expression.
    /// </summary>
    /// <typeparam name="TNode">
    /// The type of the tree node value
    /// </typeparam>
    internal class LambdaTreeNodeVisitor<TNode> : TreeNodeVistorBase<TNode>
    {
        /// <summary>
        /// The predicate.
        /// </summary>
        private readonly Expression<Func<TNode, bool>> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaTreeNodeVisitor{TNode}"/> class.
        /// </summary>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        public LambdaTreeNodeVisitor(Expression<Func<TNode, bool>> predicate)
        {
            Ensure.ParameterNotNull(predicate, "predicate was null");
            _predicate = predicate;
        }

        /// <inheritdoc/>
        public override void Visit(TreeNode<TNode> item)
        {
            throw new NotImplementedException();
        }
    }
}