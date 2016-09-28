namespace Merchello.Core.Trees
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a TreeNode Visitor.
    /// </summary>
    /// <typeparam name="TNode">
    /// The type of the tree node value
    /// </typeparam>
    internal interface ITreeNodeVisitor<TNode> : IVisitor<TreeNode<TNode>>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the visitor work has been completed.
        /// </summary>
        /// <remarks>
        /// This is used to bail out of the traversal early if we can
        /// </remarks>
        bool Completed { get; set; }
    }
}