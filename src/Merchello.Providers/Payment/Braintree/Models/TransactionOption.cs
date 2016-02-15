namespace Merchello.Providers.Payment.Braintree.Models
{
    /// <summary>
    /// The transaction option.
    /// </summary>
    public enum TransactionOption
    {
        /// <summary>
        /// The submit for settlement.
        /// </summary>
        SubmitForSettlement,

        /// <summary>
        /// The authorize.
        /// </summary>
        Authorize
    }
}