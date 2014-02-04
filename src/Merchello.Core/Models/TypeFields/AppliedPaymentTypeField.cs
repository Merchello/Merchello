using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Indicates whether a shopping cart basket is either a "basket" or a "wishlist" representation
    /// </summary>
    internal class AppliedPaymentTypeField : TypeFieldMapper<AppliedPaymentType>, IAppliedPaymentTypeField
    {
        internal AppliedPaymentTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

#region Overrides TypeFieldMapper<AppliedPaymentType>

        internal override sealed void BuildCache()
        {
            AddUpdateCache(AppliedPaymentType.Credit, new TypeField("Credit", "Credit", Constants.TypeFieldKeys.AppliedPayment.CreditRecordKey));
            AddUpdateCache(AppliedPaymentType.Debit, new TypeField("Debit", "Debit", Constants.TypeFieldKeys.AppliedPayment.DebitRecordKey));
            AddUpdateCache(AppliedPaymentType.Void, new TypeField("Void", "Void", Constants.TypeFieldKeys.AppliedPayment.VoidRecordKey));
        }

        /// <summary>
        /// Returns a custom basket or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom basket</param>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(AppliedPayment[alias]);
        }

#endregion

        /// <summary>
        /// A credit applied payment
        /// </summary>
        public ITypeField Credit
        {
            get { return GetTypeField(AppliedPaymentType.Credit); }
        }

        /// <summary>
        /// A debit applied payment
        /// </summary>
        public ITypeField Debit
        {
            get { return GetTypeField(AppliedPaymentType.Debit); }
        }

        /// <summary>
        /// A void applied payment
        /// </summary>
        public ITypeField Void
        {
            get { return GetTypeField(AppliedPaymentType.Void); }
        }


        private static TypeFieldCollection AppliedPayment
        {
            get { return Fields.AppliedPayment; }
        }

    }
}
