using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    public class PaymentMethodTypeField : TypeFieldProxyBase
    {
        /// <summary>
        /// Indicates the payment method is cash
        /// </summary>
        public static ITypeField Cash
        {
            get { return TypeFieldProvider.GetTypeField(MerchelloType.PaymentMethodCash); }
        }

        /// <summary>
        /// Indicates the payment method is credit card
        /// </summary>
        public static ITypeField CreditCard
        {
            get { return TypeFieldProvider.GetTypeField(MerchelloType.PaymentMethodCreditCard); }
        }

        /// <summary>
        /// Indicates the payment method is purchase order
        /// </summary>
        public static ITypeField PurchaseOrder
        {
            get { return TypeFieldProvider.GetTypeField(MerchelloType.PaymentMethodPurchaseOrder); }
        }

        /// <summary>
        /// Returns a custom payment methods or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom payment method</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        public new static ITypeField Custom(string alias)
        {
            return GetTypeField(PaymentMethods[alias]);
        }

        private static TypeFieldCollection PaymentMethods
        {
            get { return Fields.PaymentMethod; }
        }
    }
}
