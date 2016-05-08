namespace Merchello.Implementation
{
    /// <summary>
    /// Checkout stages.
    /// </summary>
    public enum CheckoutStage
    {
        /// <summary>
        /// Special case stage indicating either the beginning or ending of the checkout process.
        /// </summary>
        None,

        /// <summary>
        /// The billing address stage.
        /// </summary>
        BillingAddress,

        /// <summary>
        /// The shipping address stage.
        /// </summary>
        ShippingAddress,

        /// <summary>
        /// Ship rate quote stage.
        /// </summary>
        ShipRateQuote,

        /// <summary>
        /// Payment method selection stage.
        /// </summary>
        PaymentMethod,

        /// <summary>
        /// The payment payment.
        /// </summary>
        Payment,

        /// <summary>
        /// Designates a custom checkout stage
        /// </summary>
        Custom
    }
}