using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Indicates whether a shopping cart basket is either a "basket" or a "wishlist" representation
    /// </summary>
    internal class ItemCacheTypeField : TypeFieldMapper<ItemCacheType>, IItemCacheTypeField
    {
        internal ItemCacheTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

#region Overrides TypeFieldMapper<BasketType>

        internal override sealed void BuildCache()
        {
            AddUpdateCache(ItemCacheType.Basket, new TypeField("Basket", "Standard Basket", Constants.TypeFieldKeys.ItemCache.BasketKey));
            AddUpdateCache(ItemCacheType.Wishlist, new TypeField("Wishlist", "Wishlist", Constants.TypeFieldKeys.ItemCache.WishlistKey));
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
        /// Default ecommerce basket
        /// </summary>
        public ITypeField Basket
        {
            get { return GetTypeField(ItemCacheType.Basket); }
        }

        /// <summary>
        /// Wishlist
        /// </summary>
        public ITypeField Wishlist
        {
            get { return GetTypeField(ItemCacheType.Wishlist); }
        }


        private static TypeFieldCollection Baskets
        {
            get { return Fields.CustomerItemCache; }
        }

    }
}
