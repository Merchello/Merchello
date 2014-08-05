using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Logging;

namespace Merchello.Web.Workflow
{
    /// <summary>
    /// A Backoffice for the Backoffice
    /// </summary>
    public class Backoffice : IBackoffice
    {
        /// <summary>
        /// The item cache responsible for persisting the backoffice contents.
        /// </summary>
        private readonly IItemCache _itemCache;

        /// <summary>
        /// The customer.
        /// </summary>
        private readonly ICustomerBase _customer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Backoffice"/> class.
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        internal Backoffice(IItemCache itemCache, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(itemCache, "ItemCache");
            Mandate.ParameterCondition(itemCache.ItemCacheType == ItemCacheType.Backoffice, "itemCache");
            Mandate.ParameterNotNull(customer, "customer");

            _customer = customer;

            _itemCache = itemCache;
        }

        /// <summary>
        /// Gets the version of the backoffice
        /// </summary>
        public Guid VersionKey
        {
            get { return _itemCache.VersionKey; }
            internal set { _itemCache.VersionKey = value; }
        }

        /// <summary>
        /// Gets the customer associated with the backoffice
        /// </summary>
        public ICustomerBase Customer
        {
            get { return _customer; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public LineItemCollection Items
        {
            get { return _itemCache.Items; }
        }

        /// <summary>
        /// Gets the backoffice's item count
        /// </summary>
        public int TotalItemCount
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Gets the sum of all backoffice item quantities
        /// </summary>
        public int TotalQuantityCount
        {
            get { return Items.Sum(x => x.Quantity); }
        }

        /// <summary>
        /// Gets the sum of all backoffice item "amount" (price)
        /// </summary>
        public decimal TotalBackofficePrice
        {
            get { return Items.Sum(x => (x.Quantity * x.Price)); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the backoffice contains any items
        /// </summary>
        public bool IsEmpty
        {
            get { return !Items.Any(); }
        }

        /// <summary>
        /// Gets the <see cref="IItemCache"/>
        /// </summary>
        internal IItemCache ItemCache
        {
            get { return _itemCache; }
        }
  
        /// <summary>
        /// Static method to instantiate a customer's backoffice
        /// </summary>
        /// <param name="customer">The customer associated with the backoffice</param>
        /// <returns>The customer's <see cref="IBackoffice"/></returns>
        public static IBackoffice GetBackoffice(ICustomerBase customer)
        {
            return GetBackoffice(MerchelloContext.Current, customer);
        }

        /// <summary>
        /// Refreshes the runtime cache
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="backoffice">The <see cref="IBackoffice"/></param>
        public static void Refresh(IMerchelloContext merchelloContext, IBackoffice backoffice)
        {
            var cacheKey = MakeCacheKey(backoffice.Customer);
            merchelloContext.Cache.RuntimeCache.ClearCacheItem(cacheKey);

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(backoffice.Customer, ItemCacheType.Backoffice);
            backoffice = new Backoffice(customerItemCache, backoffice.Customer);
            merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => backoffice);
        }
       
        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the backoffice item collection
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added to the backoffice</param>
        public void AddItem(IProduct product)
        {
            AddItem(product, product.Name, 1);
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the backoffice item collection
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void AddItem(IProduct product, int quantity)
        {
            AddItem(product, product.Name, quantity);
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the backoffice item collection
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="name">The name to be used in the line item</param>
        /// <param name="quantity">The quantity to be added</param>
        public void AddItem(IProduct product, string name, int quantity)
        {
            AddItem(product, name, quantity, new ExtendedDataCollection());
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the backoffice item collection
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
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
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        public void AddItem(IProductVariant productVariant)
        {
            AddItem(productVariant, productVariant.Name, 1);  
        }

        /// <summary>
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void AddItem(IProductVariant productVariant, int quantity)
        {
            AddItem(productVariant, productVariant.Name, quantity);
        }

        /// <summary>
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void AddItem(IProductVariant productVariant, string name, int quantity)
        {
            AddItem(productVariant, name, quantity, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        public void AddItem(IProductVariant productVariant, string name, int quantity, ExtendedDataCollection extendedData)
        {
            if (!extendedData.DefinesProductVariant()) extendedData.AddProductVariantValues(productVariant);

            var onSale = productVariant.OnSale ? extendedData.GetSalePriceValue() : extendedData.GetPriceValue();

            AddItem(string.IsNullOrEmpty(name) ? productVariant.Name : name, productVariant.Sku, quantity, onSale, extendedData); 
        }

        /// <summary>
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        public void AddItem(string name, string sku, decimal price)
        {
            AddItem(name, sku, 1, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        public void AddItem(string name, string sku, int quantity, decimal price)
        {
            AddItem(name, sku, quantity, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the backoffice
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        public void AddItem(string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData)
        {
            if (quantity <= 0) quantity = 1;
            if (price < 0) price = 0;
            _itemCache.AddItem(LineItemType.Product, name, sku, quantity, price, extendedData);
        }

        /// <summary>
        /// Updates a backoffice item's quantity
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void UpdateQuantity(IProductVariant productVariant, int quantity)
        {
            UpdateQuantity(productVariant.Sku, quantity);
        }

        /// <summary>
        /// Updates a backoffice item's quantity
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void UpdateQuantity(Guid key, int quantity)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Key == key);

            if (item != null) UpdateQuantity(item.Sku, quantity);
        }

        /// <summary>
        /// Updates a backoffice item's quantity
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
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
        /// Removes a backoffice line item
        /// </summary>
        /// <param name="itemKey">
        /// The item Key.
        /// </param>
        public void RemoveItem(Guid itemKey)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Key == itemKey);

            if (item != null) RemoveItem(item.Sku);
        }        

        /// <summary>
        /// Removes a backoffice line item
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        public void RemoveItem(IProductVariant productVariant)
        {
            RemoveItem(productVariant.Sku);
        }

        /// <summary>
        /// Removes a backoffice line item
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public void RemoveItem(string sku)
        {
            LogHelper.Debug<Backoffice>("Before Attempting to remove - count: " + _itemCache.Items.Count);
            LogHelper.Debug<Backoffice>("Attempting to remove sku: " + sku);
            _itemCache.Items.RemoveItem(sku);            
            LogHelper.Debug<Backoffice>("After Attempting to remove - count: " + _itemCache.Items.Count);
        }

        /// <summary>
        /// Empties the backoffice
        /// </summary>
        public void Empty()
        {
            Empty(MerchelloContext.Current, this);
        }       

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        public void Refresh()
        {
           Refresh(MerchelloContext.Current, this);
        }

        /// <summary>
        /// Saves the backoffice
        /// </summary>
        public void Save()
        {
            Save(MerchelloContext.Current, this);
        }

        /// <summary>
        /// Accepts visitor class to visit backoffice items
        /// </summary>
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> to walk the collection</param>
        public void Accept(ILineItemVisitor visitor)
        {
            _itemCache.Items.Accept(visitor);
        }


        /// <summary>
        /// Instantiates a backoffice
        /// </summary>
        /// <param name="merchelloContext">The merchello context</param>
        /// <param name="customer">The customer associated with the backoffice</param>
        /// <returns>The <see cref="IBackoffice"/></returns>
        internal static IBackoffice GetBackoffice(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(customer, "customer");

            var cacheKey = MakeCacheKey(customer);

            var backoffice = (IBackoffice)merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey);

            if (backoffice != null) return backoffice;

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithKey(customer, ItemCacheType.Backoffice);

            backoffice = new Backoffice(customerItemCache, customer);

            merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey, () => backoffice);

            return backoffice;
        }

        /// <summary>
        /// Instantiates the backoffice
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <returns>
        /// The <see cref="IBackoffice"/>.
        /// </returns>
        internal static IBackoffice GetBackoffice(ICustomerBase customer, IItemCache itemCache)
        {
            return new Backoffice(itemCache, customer);
        }

        /// <summary>
        /// Empties the backoffice.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="backoffice">
        /// The backoffice.
        /// </param>
        internal static void Empty(IMerchelloContext merchelloContext, IBackoffice backoffice)
        {
            backoffice.Items.Clear();
            Save(merchelloContext, backoffice);
        }

        /// <summary>
        /// Saves the backoffice.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="backoffice">
        /// The backoffice.
        /// </param>
        internal static void Save(IMerchelloContext merchelloContext, IBackoffice backoffice)
        {
            // Update the backoffice item cache version so that it can be validated in the checkout
            ((Backoffice)backoffice).VersionKey = Guid.NewGuid();

            merchelloContext.Services.ItemCacheService.Save(((Backoffice)backoffice).ItemCache);

            Refresh(merchelloContext, backoffice);
        }
     
        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="Backoffice"/>
        /// </summary>
        /// <param name="customer">The <see cref="ICustomerBase"/></param>        
        /// <returns>The cache key for the customer's backoffice</returns>
        private static string MakeCacheKey(ICustomerBase customer)
        {
            // the version key here is not important since there can only ever be one backoffice
            return CacheKeys.ItemCacheCacheKey(customer.Key, EnumTypeFieldConverter.ItemItemCache.Basket.TypeKey, Guid.Empty);
        }
    }
}
