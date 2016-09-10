namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for configurations related to Merchello "CheckoutContext". .
    /// </summary>
    public interface ICheckoutContextSection : IMerchelloSection
    {
        /// <summary>
        /// Gets the default invoice number prefix to be used when generating invoice numbers.
        /// </summary>
        string InvoiceNumberPrefix { get; }

        /// <summary>
        /// Gets a value indicating whether taxes should be applied on the invoice.
        /// </summary>
        bool ApplyTaxesToInvoice { get; }

        /// <summary>
        /// Gets a value indicating whether the CheckoutManager should allow the <see cref="ICustomerService"/> to raise event
        /// where saving customer information.
        /// <para>
        /// In some situations, like when the customer is synced with an CRM system for example, raising a bunch of events
        /// during checkout is not desired. 
        /// </para>
        /// </summary>
        bool RaiseCustomerEvents { get; }

        /// <summary>
        /// Gets a value indicating whether reset the CheckoutManager.CustomerManager data when the basket version changes.
        /// </summary>
        bool ResetCustomerManagerDataOnVersionChange { get; }

        /// <summary>
        /// Gets a value indicating whether reset the CheckoutManager.PaymentManager data when the basket version changes.
        /// </summary>
        bool ResetPaymentManagerDataOnVersionChange { get; }

        /// <summary>
        /// Gets a value indicating whether reset the CheckoutManager.ExtendedManager data when the basket version changes.
        /// <para>
        /// e.g. Notes etc.
        /// </para>
        /// </summary>
        bool ResetExtendedManagerDataOnVersionChange { get; }

        /// <summary>
        /// Gets a value indicating whether reset the CheckoutManager.ShippingManager data when the basket version changes.
        /// </summary>
        bool ResetShippingManagerDataOnVersionChange { get; }

        /// <summary>
        /// Gets a value indicating whether reset the CheckoutManager.OfferManager data when the basket version changes.
        /// </summary>
        bool ResetOfferManagerDataOnVersionChange { get; }

        /// <summary>
        /// Gets a value indicating whether or not to empty the basket when a customer payment has been received successfully.
        /// </summary>
        bool EmptyBasketOnPaymentSuccess { get; }
    }
}