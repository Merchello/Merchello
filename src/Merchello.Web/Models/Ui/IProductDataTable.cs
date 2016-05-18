namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a product data type.
    /// </summary>
    /// <typeparam name="TProductDataTableRow">
    /// The type of the <see cref="IProductDataTableRow"/>
    /// </typeparam>
    /// <remarks>
    /// Used to preload JavaScript pricing table to reduce the number of roundtrip AJAX transactions
    /// when selecting product variants to add to the basket
    /// </remarks>
    public interface IProductDataTable<TProductDataTableRow> : IUiModel
        where TProductDataTableRow : class, IProductDataTableRow, new()
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        IEnumerable<TProductDataTableRow> Rows { get; set; }
    }
}