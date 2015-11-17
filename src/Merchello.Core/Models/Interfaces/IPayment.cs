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
        /// The key of the customer associated with the Payment
        /// </summary>
        [DataMember]
        Guid? CustomerKey { get; set; }
            
        /// <summary>
        /// The payment method key for the payment provider
        /// </summary>
        [DataMember]
        Guid? PaymentMethodKey { get; set;}
            
        /// <summary>
        /// The paymentTypeFieldKey for the payment
        /// </summary>
        [DataMember]
        Guid PaymentTypeFieldKey { get; set;}
            
        /// <summary>
        /// The name of the payment method for the payment
        /// </summary>
        [DataMember]
        string PaymentMethodName { get; set;}
            
        /// <summary>
        /// The reference number for the payment
        /// </summary>
        [DataMember]
        string ReferenceNumber { get; set;}
            
        /// <summary>
        /// The amount for the payment
        /// </summary>
        [DataMember]
        decimal Amount { get; set;}

        /// <summary>
        /// True/False indicating whether or not this payment has been authorized with the payment gateway provider
        /// </summary>
        [DataMember]
        bool Authorized { get; set; }
        
        /// <summary>
        /// True/False indicating whether or not this payment has been collected by the merchant
        /// </summary>
        [DataMember]
        bool Collected { get; set; }

        /// <summary>
        /// True/false indicating whether or not this payment has been voided
        /// </summary>
        [DataMember]
        bool Voided { get; set; }

        /// <summary>
        /// True/false indicating whether or not this payment has be exported to another system
        /// </summary>
        [DataMember]
        bool Exported { get; set;}

    }
}



