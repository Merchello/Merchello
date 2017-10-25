namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Model for adding unknown products to an invoice
    /// </summary>
    public class InvoiceAddItems
    {
        /// <summary>
        /// The invoice key
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Is this adding new pro
        /// </summary>
        public bool IsAddProduct { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<InvoiceAddItem> Items { get; set; }
    }
}