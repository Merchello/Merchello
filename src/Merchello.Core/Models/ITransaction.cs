using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Merchello Transaction object interface
    /// </summary>
    public interface ITransaction : IIdEntity
    {
            
        /// <summary>
        /// The paymentId for the Transaction
        /// </summary>
        [DataMember]
        int PaymentId { get;}

        /// <summary>
        /// The id of the invoice associated with this transaction
        /// </summary>
        [DataMember]
        int InvoiceId { get;  }
            
        /// <summary>
        /// The type for the Transaction
        /// </summary>
        [DataMember]
        Guid TransactionTypeFieldKey { get; }
            
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
    }
}



