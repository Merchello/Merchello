namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents an Applied Payment
    /// </summary>
    public interface IAppliedPayment : IEntity
    {
        /// <summary>
        /// Gets the payment key
        /// </summary>
        [DataMember]
        Guid PaymentKey { get; }

        /// <summary>
        /// Gets the invoice key of the invoice associated with this transaction.
        /// </summary>
        [DataMember]
        Guid InvoiceKey { get; }
            
        /// <summary>
        /// Gets the type field for the Applied Payment
        /// </summary>
        [DataMember]
        Guid AppliedPaymentTfKey { get; }
            
        /// <summary>
        /// Gets or sets the description for the transaction.
        /// </summary>
        [DataMember]
        string Description { get; set; }
            
        /// <summary>
        /// Gets or sets the amount of the Transaction
        /// </summary>
        [DataMember]
        decimal Amount { get; set; }
            
        /// <summary>
        /// Gets or sets a value indicating whether or not the transaction has been exported.
        /// </summary>
        /// <remarks>
        /// This is not actually used by Merchello but is useful in some implementations.
        /// </remarks>
        [DataMember]
        bool Exported { get; set; }


        /// <summary>
        /// Gets or sets the transaction type associated with this transaction.
        /// </summary>
        [DataMember]
        AppliedPaymentType TransactionType { get; set; }
    }
}



