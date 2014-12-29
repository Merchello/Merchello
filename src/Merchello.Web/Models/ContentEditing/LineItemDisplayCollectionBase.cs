namespace Merchello.Web.Models.ContentEditing
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Web.Visitors;

    /// <summary>
    /// The line item display collection base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of line item
    /// </typeparam>
    public abstract class LineItemDisplayCollectionBase<T> 
        where T : LineItemDisplayBase
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public abstract IEnumerable<T> Items { get; set; }
    }
    
    /// <summary>
    /// The line item display collection extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here.")]
    public static class LineItemDisplayCollectionExtensions
    {
        /// <summary>
        /// The accept.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        /// <typeparam name="T">
        /// The type of line item
        /// </typeparam>
        public static void Accept<T>(this LineItemDisplayCollectionBase<T> collection, ILineItemDisplayVisitor visitor)
            where T : LineItemDisplayBase
        {
            foreach (var item in collection.Items)
            {
                visitor.Visit(item);
            }
        }
    }
}