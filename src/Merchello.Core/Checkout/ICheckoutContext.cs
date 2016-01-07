namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines a checkout context.
    /// </summary>
    public interface ICheckoutContext
    {
        /// <summary>
        /// Gets the <see cref="IItemCache"/>.
        /// </summary>
        /// <remarks>
        /// This is a temporary collection of line items that is copied from the basket that can be modified
        /// while preparing the final invoice.
        /// </remarks>
        IItemCache ItemCache { get; }

        /// <summary>
        /// Gets the customer associated with the checkout.
        /// </summary>
        ICustomerBase Customer { get; }

        /// <summary>
        /// Gets the checkout version key.
        /// </summary>
        Guid VersionKey { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to apply taxes to generated invoice.
        /// </summary>
        bool ApplyTaxesToInvoice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether raise customer events.
        /// </summary>
        /// <remarks>
        /// In some implementations, there may be quite a few saves to the customer record.  Use case for setting this to 
        /// false would be an API notification on a customer record change to prevent spamming of the notification.
        /// </remarks>
        bool RaiseCustomerEvents { get; set; }
    }
}