using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class PaymentMethodTypeField : TypeFieldMapper<PaymentMethodType>, IPaymentMethodTypeField
    {
        internal PaymentMethodTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {
            AddUpdateCache(PaymentMethodType.Cash, new TypeField("Cash", "Cash", Constants.TypeFieldKeys.PaymentMethod.CashKey));
            AddUpdateCache(PaymentMethodType.CreditCard, new TypeField("CreditCard", "Credit Card", Constants.TypeFieldKeys.PaymentMethod.CreditCardKey));
            AddUpdateCache(PaymentMethodType.PurchaseOrder, new TypeField("PurchaseOrder", "Purchase Order", Constants.TypeFieldKeys.PaymentMethod.PurchaseOrderKey));
        }

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return PaymentMethods.GetTypeFields().Select(GetTypeField);
            }
        }


        /// <summary>
        /// Returns a custom payment methods or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom payment method</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(PaymentMethods[alias]);
        }

        /// <summary>
        /// Indicates the payment method is cash
        /// </summary>
        public ITypeField Cash
        {
            get { return GetTypeField(PaymentMethodType.Cash); }
        }

        /// <summary>
        /// Indicates the payment method is credit card
        /// </summary>
        public ITypeField CreditCard
        {
            get { return GetTypeField(PaymentMethodType.CreditCard); }
        }

        /// <summary>
        /// Indicates the payment method is purchase order
        /// </summary>
        public ITypeField PurchaseOrder
        {
            get { return GetTypeField(PaymentMethodType.PurchaseOrder); }
        }


        private static TypeFieldCollection PaymentMethods
        {
            get { return Fields.PaymentMethod; }
        }


    }
}
