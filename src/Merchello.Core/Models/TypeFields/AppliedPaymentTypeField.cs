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
            AddUpdateCache(AppliedPaymentType.Credit, new TypeField("Credit", "Credit", new Guid("020F6FF8-1F66-4D90-9FF4-C32A7A5AB32B")));
            AddUpdateCache(AppliedPaymentType.Debit, new TypeField("Debit", "Debit", new Guid("916929F0-96FB-430A-886D-F7A83E9A4B9A")));
            AddUpdateCache(AppliedPaymentType.Void, new TypeField("Void", "Void", new Guid("F59C7DA6-8252-4891-A5A2-7F6C38766649")));
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
