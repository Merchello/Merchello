using Merchello.Core.Models;

namespace Merchello.Core
{
    public static class InvoiceExtensions
    {
        /// <summary>
        /// Utility extension method to add an <see cref="IAddress"/> to an <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to which to set the address information</param>
        /// <param name="address">The billing address <see cref="IAddress"/></param>
        public static void SetBillingAddress(this IInvoice invoice, IAddress address)
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

        /// <summary>
        /// Utility extension to extract the billing <see cref="IAddress"/> from an <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public static IAddress GetBillingAddress(this IInvoice invoice)
        {
            return new Address()
                {
                    Name = invoice.BillToName,
                    Organization = invoice.BillToCompany,
                    Address1 = invoice.BillToAddress1,
                    Address2 = invoice.BillToAddress2,
                    Locality = invoice.BillToLocality,
                    Region = invoice.BillToRegion,
                    PostalCode = invoice.BillToPostalCode,
                    CountryCode = invoice.BillToCountryCode,
                    Phone = invoice.BillToPhone,
                    Email = invoice.BillToEmail
                };
        }         
    }
}