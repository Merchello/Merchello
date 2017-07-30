namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    using NodaMoney;

    /// <summary>
    /// Represents a payment.
    /// </summary>
    public interface IPayment : IHasExtendedData, IEntity
    {                                               
        /// <summary>
        /// Gets or sets the key of the customer associated with the Payment
        /// </summary>
        
        Guid? CustomerKey { get; set; }
            
        /// <summary>
        /// Gets or sets the payment method key for the payment provider
        /// </summary>
        
        Guid? PaymentMethodKey { get; set; }
            
        /// <summary>
        /// Gets or sets the payment type field key for the payment
        /// </summary>
        
        Guid PaymentTypeFieldKey { get; set; }
            
        /// <summary>
        /// Gets or sets the name of the payment method for the payment
        /// </summary>
        
        string PaymentMethodName { get; set; }
            
        /// <summary>
        /// Gets or sets the reference number for the payment
        /// </summary>
        
        string ReferenceNumber { get; set; }
            
        /// <summary>
        /// Gets or sets the amount for the payment
        /// </summary>
        
        Money Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been authorized with the payment gateway provider
        /// </summary>
        
        bool Authorized { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been collected by the merchant
        /// </summary>
        
        bool Collected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been voided
        /// </summary>
        
        bool Voided { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has be exported to another system
        /// </summary>
        
        bool Exported { get; set; }
    }
}



