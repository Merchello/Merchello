namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    using NodaMoney;

    /// <summary>
    /// Represents an payment applied to an invoice.
    /// </summary>
    public interface IAppliedPayment : IEntity
    {
        /// <summary>
        /// Gets the paymentId for the Transaction
        /// </summary>
        
        Guid PaymentKey { get; }

        /// <summary>
        /// Gets the id of the invoice associated with this transaction
        /// </summary>
        
        Guid InvoiceKey { get; }
            
        /// <summary>
        /// Gets the type field for the Applied Payment
        /// </summary>
        
        Guid AppliedPaymentTfKey { get; }
            
        /// <summary>
        /// Gets or sets the description for the Transaction
        /// </summary>
        
        string Description { get; set; }
            
        /// <summary>
        /// Gets or sets the amount for the Transaction
        /// </summary>
        
        Money Amount { get; set; }
            
        /// <summary>
        /// Gets or sets a value indicating whether if this record has been exported
        /// </summary>
        /// <remarks>
        /// Not by Merchello internally and can be safely used for custom implementations
        /// </remarks>
        
        bool Exported { get; set; }


        /// <summary>
        /// Gets or sets the transaction type associated with this transaction
        /// </summary>
        
        AppliedPaymentType TransactionType { get; set; }
    }
}



