namespace Merchello.Plugin.Payments.Braintree.Models
{
    using Merchello.Core.Models;

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
        /// Gets or sets the avs error response code.
        /// </summary>
        public string AvsErrorResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the avs postal code response code.
        /// </summary>
        public string AvsPostalCodeResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public BillingAddress BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the avs street address response code.
        /// </summary>
        public string AvsStreetAddressResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the masked number.
        /// </summary>
        public string MaskedNumber { get; set; }

        /// <summary>
        /// Gets or sets the currency iso code.
        /// </summary>
        public string CurrencyIsoCode { get; set; }

    }
}