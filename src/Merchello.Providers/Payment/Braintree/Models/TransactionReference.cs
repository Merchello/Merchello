namespace Merchello.Providers.Payment.Braintree.Models
{
    /// <summary>
    /// The transaction reference.
    /// </summary>
    public class TransactionReference
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the AVS error response code.
        /// </summary>
        public string AvsErrorResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the AVS postal code response code.
        /// </summary>
        public string AvsPostalCodeResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public BillingAddress BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the AVS street address response code.
        /// </summary>
        public string AvsStreetAddressResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the masked number.
        /// </summary>
        public string MaskedNumber { get; set; }

        /// <summary>
        /// Gets or sets the currency ISO code.
        /// </summary>
        public string CurrencyIsoCode { get; set; }

    }
}