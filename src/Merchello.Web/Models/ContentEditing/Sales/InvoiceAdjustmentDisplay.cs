namespace Merchello.Web.Models.ContentEditing.Sales
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Invoice adjustment operation.
    /// </summary>
    public class InvoiceAdjustmentDisplay
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<AdjustmentLineItemReference> Items { get; set; }  
    }
}