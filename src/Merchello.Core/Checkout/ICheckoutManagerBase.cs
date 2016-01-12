namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Defines the base checkout workflow.
    /// </summary>
    public interface ICheckoutManagerBase : ICheckoutContextManagerBase
    {
        /// <summary>
        /// Gets the checkout manager for customer information.
        /// </summary>
        ICheckoutCustomerManager Customer { get; }

        /// <summary>
        /// Gets the checkout extended manager for custom invoicing.
        /// </summary>
        ICheckoutExtendedManager Extended { get; }

        /// <summary>
        /// Gets the checkout manager for marketing offers.
        /// </summary>
        ICheckoutOfferManager Offer { get; }

        /// <summary>
        /// Gets the checkout manager for shipping.
        /// </summary>
        ICheckoutShippingManager Shipping { get; }

        /// <summary>
        /// Gets the payment.
        /// </summary>
        ICheckoutPaymentManager Payment { get; }
    }
}