namespace Merchello.Plugin.Payments.Stripe
{
    /// <summary>
    /// Represents Stripe payment processor transaction mode
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// An Authorize transaction
        /// </summary>
        Authorize,

        /// <summary>
        /// An Authorize and Capture transaction
        /// </summary>
        AuthorizeAndCapture
    }
}