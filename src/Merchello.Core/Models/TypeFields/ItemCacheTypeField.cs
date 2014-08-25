using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

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
            AddUpdateCache(ItemCacheType.Backoffice, new TypeField("Backoffice", "Standard Backoffice", Constants.TypeFieldKeys.ItemCache.BackofficeKey));
            AddUpdateCache(ItemCacheType.Wishlist, new TypeField("Wishlist", "Wishlist", Constants.TypeFieldKeys.ItemCache.WishlistKey));
            AddUpdateCache(ItemCacheType.Checkout, new TypeField("Checkout", "Checkout", Constants.TypeFieldKeys.ItemCache.CheckoutKey));
        }

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return ItemCaches.GetTypeFields().Select(GetTypeField);
            }
        }


        /// <summary>
        /// Returns a custom basket or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom basket</param>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(ItemCaches[alias]);
        }

#endregion

        /// <summary>
        /// Default ecommerce basket item cache
        /// </summary>
        public ITypeField Basket
        {
            get { return GetTypeField(ItemCacheType.Basket); }
        }

        /// <summary>
        /// Default ecommerce basket item cache
        /// </summary>
        public ITypeField Backoffice
        {
            get { return GetTypeField(ItemCacheType.Backoffice); }
        }

        /// <summary>
        /// The Wishlist item cache
        /// </summary>
        public ITypeField Wishlist
        {
            get { return GetTypeField(ItemCacheType.Wishlist); }
        }

        /// <summary>
        /// The Checkout item cache
        /// </summary>
        public ITypeField Checkout 
        {
            get { return GetTypeField(ItemCacheType.Checkout); }
        }


        private static TypeFieldCollection ItemCaches
        {
            get { return Fields.CustomerItemCache; }
        }

    }
}
