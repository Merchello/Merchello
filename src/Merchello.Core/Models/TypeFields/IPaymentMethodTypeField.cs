namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents a PaymentMethodTypeField
    /// </summary>
    public interface IPaymentMethodTypeField : IExtendedTypeFieldMapper<PaymentMethodType>
    {
        /// <summary>
        /// Gets the cash <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Cash { get; }

        /// <summary>
        /// Gets the credit card <see cref="ITypeField"/>.
        /// </summary>
        ITypeField CreditCard { get; }

        /// <summary>
        /// Gets the purchase order <see cref="ITypeField"/>.
        /// </summary>
        ITypeField PurchaseOrder { get; }

        /// <summary>
        /// Gets the redirect <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Redirect { get; }
    }
}