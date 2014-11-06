namespace Merchello.Web.Models.ContentEditing
{
    using System.Collections.Generic;

    /// <summary>
    /// The line item display collection base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of line item
    /// </typeparam>
    /// <remarks>
    /// TODO : T nees to be constrained to some sort of LineItemDisplayBase
    /// </remarks>
    public abstract class LineItemDisplayCollectionBase<T>
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public abstract IEnumerable<T> Items { get; set; }
    }
}