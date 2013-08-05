using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    public class PaymentMethodTypeField : TypeFieldBase
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

        private static TypeFieldCollection PaymentMethods
        {
            get { return Fields.PaymentMethod; }
        }
    }
}
