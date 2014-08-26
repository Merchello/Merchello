namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// The invoice display.
    /// </summary>
    public class InvoiceDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice number prefix.
        /// </summary>
        public string InvoiceNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the po number.
        /// </summary>
        public string PoNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the invoice status key.
        /// </summary>
        public Guid InvoiceStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice status.
        /// </summary>
        public InvoiceStatusDisplay InvoiceStatus { get; set; }

        /// <summary>
        /// Gets or sets the bill to name.
        /// </summary>
        public string BillToName { get; set; }

        /// <summary>
        /// Gets or sets the bill to address 1.
        /// </summary>
        public string BillToAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the bill to address 2.
        /// </summary>
        public string BillToAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the bill to locality.
        /// </summary>
        public string BillToLocality { get; set; }

        /// <summary>
        /// Gets or sets the bill to region.
        /// </summary>
        public string BillToRegion { get; set; }

        /// <summary>
        /// Gets or sets the bill to postal code.
        /// </summary>
        public string BillToPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the bill to country code.
        /// </summary>
        public string BillToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the bill to email.
        /// </summary>
        public string BillToEmail { get; set; }

        /// <summary>
        /// Gets or sets the bill to phone.
        /// </summary>
        public string BillToPhone { get; set; }

        /// <summary>
        /// Gets or sets the bill to company.
        /// </summary>
        public string BillToCompany { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether archived.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        public IEnumerable<OrderDisplay> Orders { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IEnumerable<InvoiceLineItemDisplay> Items { get; set; }
    }

    /// <summary>
    /// The invoice display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class InvoiceDisplayExtensions
    {

        /// <summary>
        /// Utility extension method to add an <see cref="IAddress"/> to an <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to which to set the address information</param>
        /// <param name="address">The billing address <see cref="IAddress"/></param>
        public static void SetBillingAddress(this InvoiceDisplay invoice, IAddress address)
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
        /// <param name="invoice">The invoice</param>
        /// <returns>
        /// The billing address saved in the invoice
        /// </returns>
        public static IAddress GetBillingAddress(this InvoiceDisplay invoice)
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