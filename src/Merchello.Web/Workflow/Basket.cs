using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Logging;

namespace Merchello.Web.Workflow
{
    public class Basket : IBasket
    {
        private readonly IItemCache _itemCache;
        private readonly ICustomerBase _customer;

        internal Basket(IItemCache itemCache, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(itemCache, "ItemCache");
            Mandate.ParameterCondition(itemCache.ItemCacheType == ItemCacheType.Basket, "itemCache");
            Mandate.ParameterNotNull(customer, "customer");

            _customer = customer;
            _itemCache = itemCache;
        }

        public static IBasket GetBasket(ICustomerBase customer)
        {
            return GetBasket(MerchelloContext.Current, customer);
        }

        internal static IBasket GetBasket(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(customer, "customer");

            var cacheKey = MakeCacheKey(customer);
            
            var basket = (IBasket)merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey);
            if (basket != null) return basket;

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Basket);
            basket = new Basket(customerItemCache, customer);
            merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => basket);
            
            return basket;         
        }

        // used for testing
        internal static IBasket GetBasket(ICustomerBase customer, IItemCache itemCache)
        {
            return new Basket(itemCache, customer);   
        }

        #region Overrides IBasket

        /// <summary>
        /// Gets/sets the version of the basket
        /// </summary>
        public Guid VersionKey
        {
            get { return _itemCache.VersionKey; }
            internal set { _itemCache.VersionKey = value; }
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the basket item collection
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        public void AddItem(IProduct product)
        {
           AddItem(product, product.Name, 1);
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the basket item collection
        /// </summary>
        public void AddItem(IProduct product, int quantity)
        {
            AddItem(product, product.Name, quantity);
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the basket item collection
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="name"></param>
        /// <param name="quantity"></param>
        public void AddItem(IProduct product, string name, int quantity)
        {
            AddItem(product, name, quantity, new ExtendedDataCollection());
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the basket item collection
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="name">The name of the product to be used in the line item</param>
        /// <param name="quantity">The quantity of the line item</param>
        /// <param name="extendedData">A <see cref="ExtendedDataCollection"/></param>
        public void AddItem(IProduct product, string name, int quantity, ExtendedDataCollection extendedData)
        {
            var variant = product.GetProductVariantForPurchase();
            if (variant != null)
            {
                AddItem(variant, name, quantity, extendedData);
                return;
            }
            if (!product.ProductVariants.Any()) return;

            AddItem(product.ProductVariants.First(), name, quantity, extendedData);
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(IProductVariant productVariant)
        {
            AddItem(productVariant, productVariant.Name, 1);  
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(IProductVariant productVariant, int quantity)
        {
            AddItem(productVariant, productVariant.Name, quantity);
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(IProductVariant productVariant, string name, int quantity)
        {
            AddItem(productVariant, name, quantity, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(IProductVariant productVariant, string name, int quantity, ExtendedDataCollection extendedData)
        {
            if(!extendedData.DefinesProductVariant()) extendedData.AddProductVariantValues(productVariant);

            AddItem(
                string.IsNullOrEmpty(name) ? productVariant.Name : name,
                productVariant.Sku,
                quantity,
                productVariant.OnSale ?
                extendedData.GetSalePriceValue()
                : extendedData.GetPriceValue(), extendedData); // get the price information from extended data in case it has been overriden

            //productVariant.SalePrice.Value : productVariant.Price 
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        internal void AddItem(string name, string sku, decimal price)
        {
            AddItem(name, sku, 1, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        internal void AddItem(string name, string sku, int quantity, decimal price)
        {
            AddItem(name, sku, quantity, price, new ExtendedDataCollection());
        }


        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        internal void AddItem(string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData)
        {            
            if (quantity <= 0) quantity = 1;
            if (price < 0) price = 0;
            _itemCache.AddItem(LineItemType.Product, name, sku, quantity, price, extendedData);
        }


        /// <summary>
        /// Updates a basket item's quantity
        /// </summary>
        public void UpdateQuantity(IProductVariant productVariant, int quantity)
        {
            UpdateQuantity(productVariant.Sku, quantity);
        }

        /// <summary>
        /// Updates a basket item's quantity
        /// </summary>
        public void UpdateQuantity(Guid key, int quantity)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Key == key);
            if(item != null) UpdateQuantity(item.Sku, quantity);
        }

        /// <summary>
        /// Updates a basket item's quantity
        /// </summary>
        public void UpdateQuantity(string sku, int quantity)
        {           
            if (!_itemCache.Items.Contains(sku)) return;
            if (quantity <= 0)
            {
                RemoveItem(sku);
                return;
            }
            _itemCache.Items[sku].Quantity = quantity;

        }

        /// <summary>
        /// Removes a basket line item
        /// </summary>
        public void RemoveItem(Guid itemKey)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Key == itemKey);
            if(item != null) RemoveItem(item.Sku);
        }        

        /// <summary>
        /// Removes a basket line item
        /// </summary>
        public void RemoveItem(IProductVariant productVariant)
        {
            RemoveItem(productVariant.Sku);
        }

        /// <summary>
        /// True/false indicating whether or not the basket contains any items
        /// </summary>
        public bool IsEmpty
        {
            get { return !Items.Any(); }
        }

        /// <summary>
        /// Removes a basket line item
        /// </summary>
        public void RemoveItem(string sku)
        {
            LogHelper.Debug<Basket>("Before Attempting to remove - count: " + _itemCache.Items.Count);
            LogHelper.Debug<Basket>("Attempting to remove sku: " + sku);
            _itemCache.Items.RemoveItem(sku);
            LogHelper.Debug<Basket>("After Attempting to remove - count: " + _itemCache.Items.Count);
        }

        /// <summary>
        /// Empties the basket
        /// </summary>
        public void Empty()
        {
            Empty(MerchelloContext.Current, this);
        }

        internal static void Empty(IMerchelloContext merchelloContext, IBasket basket)
        {
            basket.Items.Clear();
            Save(merchelloContext, basket);
        }

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        public void Refresh()
        {
           Refresh(MerchelloContext.Current, this);
        }

        public static void Refresh(IMerchelloContext merchelloContext, IBasket basket)
        {
            var cacheKey = MakeCacheKey(basket.Customer);
            merchelloContext.Cache.RuntimeCache.ClearCacheItem(cacheKey);

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(basket.Customer, ItemCacheType.Basket);
            basket = new Basket(customerItemCache, basket.Customer);
            merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => basket);
        }

        /// <summary>
        /// Saves the basket
        /// </summary>
        public void Save()
        {
            Save(MerchelloContext.Current, this);
        }

        internal static void Save(IMerchelloContext merchelloContext, IBasket basket)
        {
            // Update the basket item cache version so that it can be validated in the checkout
            ((Basket)basket).VersionKey = Guid.NewGuid();

            merchelloContext.Services.ItemCacheService.Save(((Basket)basket).ItemCache);
            Refresh(merchelloContext, basket);
        }

        /// <summary>
        /// Accepts visitor class to visit basket items
        /// </summary>
        /// <param name="visitor"><see cref="ILineItemVisitor"/></param>
        public void Accept(ILineItemVisitor visitor)
        {
            _itemCache.Items.Accept(visitor);
        }

        internal IItemCache ItemCache
        {
            get { return _itemCache; }
        }

        /// <summary>
        /// The customer associated with the basket
        /// </summary>
        public ICustomerBase Customer {
            get { return _customer; }
        }

        public LineItemCollection Items
        {
            get { return _itemCache.Items; }
        }

        /// <summary>
        /// Returns the basket's item count
        /// </summary>
        public int TotalItemCount 
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Returns the sum of all basket item quantities
        /// </summary>
        public int TotalQuantityCount 
        {
            get { return Items.Sum(x => x.Quantity); }
        }

        /// <summary>
        /// Returns the sum of all basket item "amount" (price)
        /// </summary>
        public decimal TotalBasketPrice
        {
            get { return Items.Sum(x => (x.Quantity * x.Price)); }
        }

        #endregion

        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="Basket"/>
        /// </summary>
        /// <param name="customer"><see cref="ICustomerBase"/></param>        
        /// <returns></returns>
        private static string MakeCacheKey(ICustomerBase customer)
        {
            // the version key here is not important since there can only ever be one basket
            return CacheKeys.ItemCacheCacheKey(customer.EntityKey, EnumTypeFieldConverter.ItemItemCache.Basket.TypeKey, Guid.Empty);
        }

        
    }
}