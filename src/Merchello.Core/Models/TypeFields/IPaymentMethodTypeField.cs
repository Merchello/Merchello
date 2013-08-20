namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines a PaymentTypeField
    /// </summary>
    public interface IPaymentMethodTypeField : ITypeFieldMapper<PaymentMethodType>
    {
        /// <summary>
        /// The cash type
        /// </summary>
        ITypeField Cash { get; } 

        /// <summary>
        /// The credit card type
        /// </summary>
        ITypeField CreditCard { get; }

        /// <summary>
        /// The purchase order type
        /// </summary>
        ITypeField PurchaseOrder { get; }
    }
}