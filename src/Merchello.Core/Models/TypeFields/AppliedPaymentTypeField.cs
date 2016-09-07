namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal class AppliedPaymentTypeField : TypeFieldMapper<AppliedPaymentType>, IAppliedPaymentTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPaymentTypeField"/> class.
        /// </summary>
        internal AppliedPaymentTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Credit
        {
            get
            {
                return GetTypeField(AppliedPaymentType.Credit);
            }
        }

        /// <inheritdoc/>
        public ITypeField Debit
        {
            get
            {
                return GetTypeField(AppliedPaymentType.Debit);
            }
        }

        /// <inheritdoc/>
        public ITypeField Void
        {
            get
            {
                return GetTypeField(AppliedPaymentType.Void);
            }
        }

        /// <inheritdoc/>
        public ITypeField Refund
        {
            get { return GetTypeField(AppliedPaymentType.Refund); }
        }

        /// <inheritdoc/>
        public ITypeField Denied
        {
            get
            {
                return GetTypeField(AppliedPaymentType.Denied);
            }
        }

        /// <inheritdoc/>
        internal sealed override void BuildCache()
        {
            AddUpdateCache(AppliedPaymentType.Credit, new TypeField("Credit", "Credit", Constants.TypeFieldKeys.AppliedPayment.CreditRecordKey));
            AddUpdateCache(AppliedPaymentType.Debit, new TypeField("Debit", "Debit", Constants.TypeFieldKeys.AppliedPayment.DebitRecordKey));
            AddUpdateCache(AppliedPaymentType.Void, new TypeField("Void", "Void", Constants.TypeFieldKeys.AppliedPayment.VoidRecordKey));
            AddUpdateCache(AppliedPaymentType.Refund, new TypeField("Refund", "Refund", Constants.TypeFieldKeys.AppliedPayment.RefundRecordKey));
            AddUpdateCache(AppliedPaymentType.Denied, new TypeField("Denied", "Denied", Constants.TypeFieldKeys.AppliedPayment.DeniedRecordKey));
        }
    }
}
