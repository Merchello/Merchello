namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Defines a customer sales history.
    /// </summary>
    public interface ICustomerSalesHistory
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

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator{InvoiceDisplay}"/>.
        /// </returns>
        IEnumerator<InvoiceDisplay> GetEnumerator();
    }
}