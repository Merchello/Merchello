namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Defines a customer sales history.
    /// </summary>
    public interface ICustomerSalesHistory : IEnumerable<InvoiceDisplay>
    {
        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets the total outstanding.
        /// </summary>
        decimal TotalOutstanding { get; }

        /// <summary>
        /// Gets the total paid.
        /// </summary>
        decimal TotalPaid { get; }

        /// <summary>
        /// Gets the total purchases.
        /// </summary>
        decimal TotalPurchases { get; }
    }
}