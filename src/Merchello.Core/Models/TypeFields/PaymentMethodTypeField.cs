using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    internal sealed class PaymentMethodTypeField : TypeFieldMapper<PaymentMethodType>, IPaymentMethodTypeField
    {
        internal PaymentMethodTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {
            AddUpdateCache(PaymentMethodType.Cash, new TypeField("Cash", "Cash", new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5")));
            AddUpdateCache(PaymentMethodType.CreditCard, new TypeField("CreditCard", "Credit Card", new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A")));
            AddUpdateCache(PaymentMethodType.PurchaseOrder, new TypeField("PurchaseOrder", "Purchase Order", new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E")));
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
