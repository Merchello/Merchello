namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.Outline;

    /// <summary>
    /// The payment method type field.
    /// </summary>
    internal sealed class PaymentMethodTypeField : TypeFieldMapper<PaymentMethodType>, IPaymentMethodTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodTypeField"/> class.
        /// </summary>
        internal PaymentMethodTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }


        /// <summary>
        /// Gets the custom type fields.
        /// </summary>
        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return PaymentMethods.GetTypeFields().Select(GetTypeField);
            }
        }

        /// <summary>
        /// Gets a value indicating the payment method is cash
        /// </summary>
        public ITypeField Cash
        {
            get { return GetTypeField(PaymentMethodType.Cash); }
        }

        /// <summary>
        /// Gets a value indicating the payment method is based off a redirect.
        /// </summary>
        public ITypeField Redirect
        {
            get
            {
                return GetTypeField(PaymentMethodType.Redirect);
            }
        }

        /// <summary>
        /// Gets a value indicating the payment method is credit card
        /// </summary>
        public ITypeField CreditCard
        {
            get { return GetTypeField(PaymentMethodType.CreditCard); }
        }

        /// <summary>
        /// Gets a value indicating the payment method is purchase order
        /// </summary>
        public ITypeField PurchaseOrder
        {
            get { return GetTypeField(PaymentMethodType.PurchaseOrder); }
        }

        /// <summary>
        /// Gets a value indicating the payment method is a customer credit.
        /// </summary>
        public ITypeField CustomerCredit
        {
            get
            {
                return GetTypeField(PaymentMethodType.CustomerCredit);
            }
        }

        /// <summary>
        /// Gets the payment methods.
        /// </summary>
        internal static TypeFieldCollection PaymentMethods
        {
            get { return Fields.PaymentMethod; }
        }

        /// <summary>
        /// The build cache.
        /// </summary>
        internal override void BuildCache()
        {
            AddUpdateCache(PaymentMethodType.Cash, new TypeField("Cash", "Cash", Constants.TypeFieldKeys.PaymentMethod.CashKey));
            AddUpdateCache(PaymentMethodType.Redirect, new TypeField("Redirect", "Redirect", Constants.TypeFieldKeys.PaymentMethod.RedirectKey));
            AddUpdateCache(PaymentMethodType.CreditCard, new TypeField("CreditCard", "Credit Card", Constants.TypeFieldKeys.PaymentMethod.CreditCardKey));
            AddUpdateCache(PaymentMethodType.PurchaseOrder, new TypeField("PurchaseOrder", "Purchase Order", Constants.TypeFieldKeys.PaymentMethod.PurchaseOrderKey));
            AddUpdateCache(PaymentMethodType.CustomerCredit, new TypeField("CustomerCreditKey", "Customer Credit", Constants.TypeFieldKeys.PaymentMethod.CustomerCreditKey));
            AddUpdateCache(PaymentMethodType.Custom, NotFound);
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
    }
}
