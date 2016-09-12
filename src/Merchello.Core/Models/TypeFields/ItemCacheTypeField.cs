namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal sealed class ItemCacheTypeField : ExtendedTypeFieldMapper<ItemCacheType>, IItemCacheTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCacheTypeField"/> class.
        /// </summary>
        internal ItemCacheTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Basket
        {
            get { return GetTypeField(ItemCacheType.Basket); }
        }

        /// <inheritdoc/>
        public ITypeField Backoffice
        {
            get { return GetTypeField(ItemCacheType.Backoffice); }
        }

        /// <inheritdoc/>
        public ITypeField Wishlist
        {
            get { return GetTypeField(ItemCacheType.Wishlist); }
        }

        /// <inheritdoc/>
        public ITypeField Checkout 
        {
            get
            {
                return GetTypeField(ItemCacheType.Checkout);
            }
        }

        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(ItemCacheType.Basket, new TypeField("Basket", "Standard Basket", Constants.TypeFieldKeys.ItemCache.BasketKey));
            AddUpdateCache(ItemCacheType.Backoffice, new TypeField("Backoffice", "Standard Backoffice", Constants.TypeFieldKeys.ItemCache.BackofficeKey));
            AddUpdateCache(ItemCacheType.Wishlist, new TypeField("Wishlist", "Wishlist", Constants.TypeFieldKeys.ItemCache.WishlistKey));
            AddUpdateCache(ItemCacheType.Checkout, new TypeField("Checkout", "Checkout", Constants.TypeFieldKeys.ItemCache.CheckoutKey));
            AddUpdateCache(ItemCacheType.Custom, NotFound);
        }
    }
}
