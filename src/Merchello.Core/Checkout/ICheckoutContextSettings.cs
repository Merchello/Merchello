namespace Merchello.Core.Checkout
{
    /// <summary>
    /// The CheckoutContextSettings interface.
    /// </summary>
    public interface ICheckoutContextSettings
    {
        /// <summary>
        /// Gets or sets the invoice number prefix to be added to the generated invoice in the invoice builder.
        /// </summary>
        string InvoiceNumberPrefix { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether reset the customer manager data on version change.
        /// </summary>
        bool ResetCustomerManagerDataOnVersionChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reset the payment manager data on version change.
        /// </summary>
        bool ResetPaymentManagerDataOnVersionChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reset the extended manager data on version change.
        /// </summary>
        bool ResetExtendedManagerDataOnVersionChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reset the shipping manager data on version change.
        /// </summary>
        bool ResetShippingManagerDataOnVersionChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reset the offer manager data on version change.
        /// </summary>
        bool ResetOfferManagerDataOnVersionChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to empty the basket on payment success.
        /// </summary>
        /// <remarks>
        /// TODO RSS - the basket is defined in .Web so this setting should probably be moved
        /// </remarks>
        bool EmptyBasketOnPaymentSuccess { get; set; }
    }
}