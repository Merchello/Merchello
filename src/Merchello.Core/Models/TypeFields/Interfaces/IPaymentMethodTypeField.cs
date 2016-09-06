namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Represents a payment method type field
    /// </summary>
    public interface IPaymentMethodTypeField : ITypeFieldMapper<PaymentMethodType>
    {
        /// <summary>
        /// Gets the cash type field
        /// </summary>
        ITypeField Cash { get; } 

        /// <summary>
        /// Gets the credit card type field
        /// </summary>
        ITypeField CreditCard { get; }

        /// <summary>
        /// Gets the purchase order type field
        /// </summary>
        ITypeField PurchaseOrder { get; }
    }
}