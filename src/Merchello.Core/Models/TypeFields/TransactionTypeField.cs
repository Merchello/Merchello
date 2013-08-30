using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Indicates whether a shopping cart basket is either a "basket" or a "wishlist" representation
    /// </summary>
    internal class TransactionTypeField : TypeFieldMapper<TransactionType>, ITransactionTypeField
    {
        internal TransactionTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

#region Overrides TypeFieldMapper<BasketType>

        internal override sealed void BuildCache()
        {
            AddUpdateCache(TransactionType.Credit, new TypeField("Credit", "Credit", new Guid("020F6FF8-1F66-4D90-9FF4-C32A7A5AB32B")));
            AddUpdateCache(TransactionType.Debit, new TypeField("Debit", "Debit", new Guid("916929F0-96FB-430A-886D-F7A83E9A4B9A")));
        }

        /// <summary>
        /// Returns a custom basket or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom basket</param>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Baskets[alias]);
        }

#endregion

        /// <summary>
        /// A credit transaction
        /// </summary>
        public ITypeField Credit
        {
            get { return GetTypeField(TransactionType.Credit); }
        }

        /// <summary>
        /// A debit transaction
        /// </summary>
        public ITypeField Debit
        {
            get { return GetTypeField(TransactionType.Debit); }
        }


        private static TypeFieldCollection Baskets
        {
            get { return Fields.Basket; }
        }

    }
}
