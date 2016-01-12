namespace Merchello.Core.Checkout
{
    /// <summary>
    /// The CheckoutContextSettings interface.
    /// </summary>
    public interface ICheckoutContextChangeSettings
    {
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