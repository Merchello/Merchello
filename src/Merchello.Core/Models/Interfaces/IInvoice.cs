namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    

    using Merchello.Core.Models.EntityBase;

    using NodaMoney;

    /// <summary>
    /// Represents an invoice.
    /// </summary>
    public interface IInvoice : IStoreSpecificEntity, ILineItemContainer, IHasNotes
    {
        /// <summary>
        /// Gets or sets the unique customer 'key' to associated with the invoice
        /// </summary>
        
        Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the optional invoice number prefix
        /// </summary>
        
        string InvoiceNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the invoice number
        /// </summary>
        
        int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the po number.
        /// </summary>
        
        string PoNumber { get; set; }

        /// <summary>
        /// Gets or sets the date the invoice was issued to customer
        /// </summary>
        
        DateTime InvoiceDate { get; set; }
        
        /// <summary>
        /// Gets the key for the invoice status associated with this invoice
        /// </summary>
        
        Guid InvoiceStatusKey { get; }

        /// <summary>
        /// Gets or sets the Invoice Status
        /// </summary>
        
        IInvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// Gets or sets the full name to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToName { get; set; }

        /// <summary>
        /// Gets or sets the address line 1 to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2 to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the city or locality to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToLocality { get; set; }

        /// <summary>
        /// Gets or sets the state, region or province to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToRegion { get; set; }

        /// <summary>
        /// Gets or sets the postal code to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the email address to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToEmail { get; set; }

        /// <summary>
        /// Gets or sets the phone number to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToPhone { get; set; }

        /// <summary>
        /// Gets or sets the company name to use for billing.  Generally copied from customer address.
        /// </summary>
        
        string BillToCompany { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether or not this invoice has been exported to an external system
        /// </summary>
        
        bool Exported { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this invoice has been archived
        /// </summary>
        
        bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the total invoice amount
        /// </summary>
        
        Money Total { get; set; }

        /// <summary>
        /// Gets or sets the collection of Orders associated with the Invoice
        /// </summary>
        
        //OrderCollection Orders { get; set; }

        /// <summary>
        /// Accepts visitor class to visit invoice line items
        /// </summary>
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> class</param>
        void Accept(ILineItemVisitor visitor);  
    }
}
