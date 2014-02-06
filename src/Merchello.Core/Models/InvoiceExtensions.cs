namespace Merchello.Core.Models
{
    public static class InvoiceExtensions
    {
        /// <summary>
        /// Utility extension method to quickly add an address to an invoice
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to which to set the address information</param>
        /// <param name="address">The billing address <see cref="IAddress"/></param>
        public static void AddBillingAddress(this IInvoice invoice, IAddress address)
        {
            invoice.BillToName = address.Name;
            invoice.BillToCompany = address.Organization;
            invoice.BillToAddress1 = address.Address1;
            invoice.BillToAddress2 = address.Address2;
            invoice.BillToLocality = address.Locality;
            invoice.BillToRegion = address.Region;
            invoice.BillToPostalCode = address.PostalCode;
            invoice.BillToCountryCode = address.CountryCode;
            invoice.BillToPhone = address.Phone;
            invoice.BillToEmail = address.Email;
        }
         
    }
}