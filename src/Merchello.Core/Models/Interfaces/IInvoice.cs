namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines an invoice.
    /// </summary>
    public interface IInvoice : ILineItemContainer
    {
        /// <summary>
        /// Gets or sets the unique customer 'key' to associated with the invoice
        /// </summary>
        [DataMember]
        Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the optional invoice number prefix
        /// </summary>
        [DataMember]
        string InvoiceNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the invoice number
        /// </summary>
        [DataMember]
        int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the po number.
        /// </summary>
        [DataMember]
        string PoNumber { get; set; }

        /// <summary>
        /// Gets or sets the date the invoice was issued to customer
        /// </summary>
        [DataMember]
        DateTime InvoiceDate { get; set; }
        
        /// <summary>
        /// Gets the key for the invoice status associated with this invoice
        /// </summary>
        [DataMember]
        Guid InvoiceStatusKey { get; }

        /// <summary>
        /// Gets or sets the Invoice Status
        /// </summary>
        [DataMember]
        IInvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// Gets or sets the full name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToName { get; set; }

        /// <summary>
        /// Gets or sets the adress line 1 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToLocality { get; set; }

        /// <summary>
        /// Gets or sets the state, region or province to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToRegion { get; set; }

        /// <summary>
        /// Gets or sets the postal code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the email address to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToEmail { get; set; }

        /// <summary>
        /// Gets or sets the phone number to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToPhone { get; set; }

        /// <summary>
        /// Gets or sets the company name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToCompany { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not this invoice has been exported to an external system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether or not this invoice has been archived
        /// </summary>
        [DataMember]
        bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the total invoice amount
        /// </summary>
        [DataMember]
        decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the collection of Orders associated with the Invoice
        /// </summary>
        [DataMember]
        OrderCollection Orders { get; set; }
    }
}
