using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;


namespace Merchello.Core.Models
{
    public interface IInvoice : IIdEntity
    {
        /// <summary>
        /// Optional id of the order assoicated with the invoice
        /// </summary>
        [DataMember]
        int? OrderId { get; set; }

        /// <summary>
        /// The invoice number
        /// </summary>
        [DataMember]
        string InvoiceNumber { get; }

        /// <summary>
        /// The date the invoice was issued to customer
        /// </summary>
        [DataMember]
        DateTime InvoiceDate { get; set; }

        /// <summary>
        /// The customer id to associated with the invoice
        /// </summary>
        [IgnoreDataMember]
        int CustomerId { get; }

        [DataMember]
        ICustomer Customer { get; }

        /// <summary>
        /// The id for the invoice status associated with this invoice
        /// </summary>
        [DataMember]
        int InvoiceStatusId { get; }

        /// <summary>
        /// The status of the invoice
        /// </summary>
        [DataMember]
        IInvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// The full name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToName { get; set; }

        /// <summary>
        /// The adress line 1 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToAddress1 { get; set; }

        /// <summary>
        /// The address line 2 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToAddress2 { get; set; }

        /// <summary>
        /// The city or locality to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToLocality { get; set; }

        /// <summary>
        /// The state, region or province to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToRegion { get; set; }

        /// <summary>
        /// The postal code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToPostalCode { get; set; }

        /// <summary>
        /// The country code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToCountryCode { get; set; }

        /// <summary>
        /// The email address to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToEmail { get; set; }

        /// <summary>
        /// The phone number to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToPhone { get; set; }

        /// <summary>
        /// The company name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        string BillToCompany { get; set; }

        /// <summary>
        /// Indicates whether or not this invoice has been exported to an external system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }

        /// <summary>
        /// Indicates whether or not this invoice has been paid in full
        /// </summary>
        [DataMember]
        bool Paid { get; set; }

        /// <summary>
        /// The total invoice amount
        /// </summary>
        [DataMember]
        decimal Amount { get; }

    }


}
