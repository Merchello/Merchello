namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a Merchello Payment 
    /// </summary>
    public interface IPayment : IHasExtendedData, IEntity
    {                                               
        /// <summary>
        /// Gets or sets the key of the customer associated with the Payment
        /// </summary>
        [DataMember]
        Guid? CustomerKey { get; set; }
            
        /// <summary>
        /// Gets or sets the payment method key for the payment provider
        /// </summary>
        [DataMember]
        Guid? PaymentMethodKey { get; set; }
            
        /// <summary>
        /// Gets or sets the payment type field key for the payment
        /// </summary>
        [DataMember]
        Guid PaymentTypeFieldKey { get; set; }
            
        /// <summary>
        /// Gets or sets the name of the payment method for the payment
        /// </summary>
        [DataMember]
        string PaymentMethodName { get; set; }
            
        /// <summary>
        /// Gets or sets the reference number for the payment
        /// </summary>
        [DataMember]
        string ReferenceNumber { get; set; }
            
        /// <summary>
        /// Gets or sets the amount for the payment
        /// </summary>
        [DataMember]
        decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been authorized with the payment gateway provider
        /// </summary>
        [DataMember]
        bool Authorized { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been collected by the merchant
        /// </summary>
        [DataMember]
        bool Collected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been voided
        /// </summary>
        [DataMember]
        bool Voided { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has be exported to another system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }

    }
}



