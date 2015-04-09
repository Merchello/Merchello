namespace Merchello.Plugin.Payments.Braintree.Models
{
    using Merchello.Core.Models;

    public class TransactionReference
    {
        public string Id { get; set; }

        public decimal Amount { get; set; }

        public string AvsErrorResponseCode { get; set; }

        public string AvsPostalCodeResponseCode { get; set; }

        public BillingAddress BillingAddress { get; set; }

        public string AvsStreetAddressResponseCode { get; set; }

        public string MaskedNumber { get; set; }

        public string CurrencyIsoCode { get; set; }

    }

    public class BillingAddress
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string StreetAddress { get; set; }
        public string ExtendedAddress { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string CountryCodeAlpha2 { get; set; }
    }
}