using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Transaction object interface
    /// </summary>
    public interface IAppliedPayment : IEntity
    {
            
        /// <summary>
        /// The paymentId for the Transaction
        /// </summary>
        [DataMember]
        Guid PaymentKey { get;}

        /// <summary>
        /// The id of the invoice associated with this transaction
        /// </summary>
        [DataMember]
        Guid InvoiceKey { get; }
            
        /// <summary>
        /// The type field for the Applied Payment
        /// </summary>
        [DataMember]
        Guid AppliedPaymentTfKey { get; }
            
        /// <summary>
        /// The description for the Transaction
        /// </summary>
        [DataMember]
        string Description { get; set;}
            
        /// <summary>
        /// The amount for the Transaction
        /// </summary>
        [DataMember]
        decimal Amount { get; set;}
            
        /// <summary>
        /// The exported for the Transaction
        /// </summary>
        [DataMember]
        bool Exported { get; set;}


        /// <summary>
        /// The transaction type associated with this transaction
        /// </summary>
        [DataMember]
        AppliedPaymentType TransactionType { get; set; }
    }
}



