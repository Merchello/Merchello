using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    public class PaymentMethodTypeField : TypeFieldProxyBase
    {
        /// <summary>
        /// Indicates the payment method is cash
        /// </summary>
        public static ITypeField Cash
        {
            get { return GetTypeField(PaymentMethods["Cash"]); }
        }

        /// <summary>
        /// Indicates the payment method is credit card
        /// </summary>
        public static ITypeField CreditCard
        {
            get { return GetTypeField(PaymentMethods["CreditCard"]); }
        }

        /// <summary>
        /// Indicates the payment method is purchase order
        /// </summary>
        public static ITypeField PurchaseOrder
        {
            get { return GetTypeField(PaymentMethods["PurchaseOrder"]); }
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
