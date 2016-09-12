namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal sealed class PaymentMethodTypeField : ExtendedTypeFieldMapper<PaymentMethodType>, IPaymentMethodTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodTypeField"/> class.
        /// </summary>
        internal PaymentMethodTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Cash
        {
            get { return GetTypeField(PaymentMethodType.Cash); }
        }

        /// <inheritdoc/>
        public ITypeField Redirect
        {
            get
            {
                return GetTypeField(PaymentMethodType.Redirect);
            }
        }

        /// <inheritdoc/>
        public ITypeField CreditCard
        {
            get { return GetTypeField(PaymentMethodType.CreditCard); }
        }

        /// <inheritdoc/>
        public ITypeField PurchaseOrder
        {
            get { return GetTypeField(PaymentMethodType.PurchaseOrder); }
        }

        /// <inheritdoc/>
        public ITypeField CustomerCredit
        {
            get
            {
                return GetTypeField(PaymentMethodType.CustomerCredit);
            }
        }

        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(PaymentMethodType.Cash, new TypeField("Cash", "Cash", Constants.TypeFieldKeys.PaymentMethod.CashKey));
            AddUpdateCache(PaymentMethodType.Redirect, new TypeField("Redirect", "Redirect", Constants.TypeFieldKeys.PaymentMethod.RedirectKey));
            AddUpdateCache(PaymentMethodType.CreditCard, new TypeField("CreditCard", "Credit Card", Constants.TypeFieldKeys.PaymentMethod.CreditCardKey));
            AddUpdateCache(PaymentMethodType.PurchaseOrder, new TypeField("PurchaseOrder", "Purchase Order", Constants.TypeFieldKeys.PaymentMethod.PurchaseOrderKey));
            AddUpdateCache(PaymentMethodType.CustomerCredit, new TypeField("CustomerCreditKey", "Customer Credit", Constants.TypeFieldKeys.PaymentMethod.CustomerCreditKey));
            AddUpdateCache(PaymentMethodType.Custom, NotFound);
        }
    }
}
