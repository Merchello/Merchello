using System;
using System.Collections.Generic;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface IInvoiceBase : IAggregateRoot
    {
        /// <summary>
        /// The invoice number
        /// </summary>
        string InvoiceNumber { get; set; }

        /// <summary>
        /// The date the invoice was issued to customer
        /// </summary>
        DateTime InvoiceDate { get; set; }

        /// <summary>
        /// The customer to associated with the invoice
        /// </summary>
        Customer Customer { get; set; }

        /// <summary>
        /// The status of the invoice
        /// </summary>
        InvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// The full name to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToName { get; set; }

        /// <summary>
        /// The adress line 1 to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToAddress1 { get; set; }

        /// <summary>
        /// The address line 2 to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToAddress2 { get; set; }

        /// <summary>
        /// The city or locality to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToLocality { get; set; }

        /// <summary>
        /// The state, region or province to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToRegion { get; set; }

        /// <summary>
        /// The postal code to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToPostalCode { get; set; }

        /// <summary>
        /// The country code to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToCountryCode { get; set; }

        /// <summary>
        /// The email address to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToEmail { get; set; }

        /// <summary>
        /// The phone number to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToPhone { get; set; }

        /// <summary>
        /// The company name to use for billing.  Generally copied from customer address.
        /// </summary>
        string BillToCompany { get; set; }

        /// <summary>
        /// Indicates whether or not this invoice has been exported to an external system
        /// </summary>
        bool Exported { get; set; }

        /// <summary>
        /// Indicates whether or not this invoice has been paid in full
        /// </summary>
        bool Paid { get; set; }

        /// <summary>
        /// Indicates whether or not all shipments have been completed for items in this invoice
        /// </summary>
        bool Shipped { get; set; }

        /// <summary>
        /// The total invoice amount
        /// </summary>
        decimal Total { get; set; }

    }


}
