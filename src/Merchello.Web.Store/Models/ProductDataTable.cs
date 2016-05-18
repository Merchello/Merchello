namespace Merchello.Web.Store.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// The pricing table.
    /// </summary>
    /// <remarks>
    /// Used to preload JavaScript pricing table to reduce the number of roundtrip AJAX transactions
    /// when selecting product variants to add to the basket
    /// </remarks>
    public class ProductDataTable : IProductDataTable<ProductDataTableRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDataTable"/> class.
        /// </summary>
        public ProductDataTable()
        {
            this.Rows = Enumerable.Empty<ProductDataTableRow>();
        }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        public IEnumerable<ProductDataTableRow> Rows { get; set; }
    }
}