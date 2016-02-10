namespace Merchello.Providers.Payment.Braintree.Models
{
    /// <summary>
    /// The billing address.
    /// </summary>
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