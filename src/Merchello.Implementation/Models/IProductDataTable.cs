namespace Merchello.Implementation.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a product data type.
    /// </summary>
    /// <remarks>
    /// Used to preload JavaScript pricing table to reduce the number of roundtrip AJAX transactions
    /// when selecting product variants to add to the basket
    /// </remarks>
    public interface IProductDataTable
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        IEnumerable<ProductDataTableRow> Rows { get; set; }
    }
}