namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Merchello Transaction object interface
    /// </summary>
    public interface IAppliedPayment : IEntity
    {
            
        /// <summary>
        /// Gets the paymentId for the Transaction
        /// </summary>
        [DataMember]
        Guid PaymentKey { get; }

        /// <summary>
        /// Gets the id of the invoice associated with this transaction
        /// </summary>
        [DataMember]
        Guid InvoiceKey { get; }
            
        /// <summary>
        /// Gets the type field for the Applied Payment
        /// </summary>
        [DataMember]
        Guid AppliedPaymentTfKey { get; }
            
        /// <summary>
        /// Gets or sets the description for the Transaction
        /// </summary>
        [DataMember]
        string Description { get; set; }
            
        /// <summary>
        /// Gets or sets the amount for the Transaction
        /// </summary>
        [DataMember]
        decimal Amount { get; set; }
            
        /// <summary>
        /// Gets or sets a value indicating whether if this record has been exported
        /// </summary>
        /// <remarks>
        /// Not by Merchello internally and can be safely used for custom implementations
        /// </remarks>
        [DataMember]
        bool Exported { get; set; }


        /// <summary>
        /// Gets or sets the transaction type associated with this transaction
        /// </summary>
        [DataMember]
        AppliedPaymentType TransactionType { get; set; }
    }
}



