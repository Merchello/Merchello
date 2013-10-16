using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Web.Cache;

namespace Merchello.Web.Models
{
    public class Basket : IBasket
    {
        private readonly IItemCache _itemCache;
        private readonly ICustomerBase _customer;

        internal Basket(IItemCache itemCache, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(itemCache, "ItemCache");
            Mandate.ParameterNotNull(customer, "customer");

            _customer = customer;
            _itemCache = itemCache;
        }

        public static IBasket GetBasket(ICustomerBase customer)
        {
            return GetBasket(MerchelloContext.Current, customer);
        }

        public static IBasket GetBasket(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(customer, "customer");

            var cacheKey = MakeCacheKey(customer);
            
            var basket = (IBasket)merchelloContext.Cache.RuntimeCache.GetCacheItem(cacheKey);
            if (basket != null) return basket;

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithId(customer, ItemCacheType.Basket);
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
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="name"></param>
        /// <param name="quantity"></param>
        public void AddItem(IProduct product, string name, int quantity)
        {
            var variant = product.GetProductVariantForPurchase();
            if (variant != null)
            {
                AddItem(variant, name, quantity);
                return;
            }
            if (!product.ProductVariants.Any()) return;

            AddItem(product.ProductVariants.First(), name, quantity);
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
        public void AddItem(IProductVariant productVariant, string name, int quantity)
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.AddProductVariantValues(productVariant);

            AddItem(
                string.IsNullOrEmpty(name) ? productVariant.Name : name, 
                productVariant.Sku, 
                quantity,
                productVariant.OnSale ?
                productVariant.SalePrice != null ? productVariant.SalePrice.Value : productVariant.Price
                : productVariant.Price, extendedData);
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(string name, string sku, decimal price)
        {
            AddItem(name, sku, 1, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(string name, string sku, int quantity, decimal price)
        {
            AddItem(name, sku, quantity, price, new ExtendedDataCollection());
        }


        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData)
        {
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
        public void UpdateQuantity(int id, int quantity)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Id == id);
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
        public void RemoveItem(int id)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Id == id);
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
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
            if(_itemCache.Items.Contains(sku)) _itemCache.Items.RemoveItem(sku);
        }

        /// <summary>
        /// Empties the basket
        /// </summary>
        public void Empty()
        {
            _itemCache.Items.Clear();
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

            var customerItemCache = merchelloContext.Services.ItemCacheService.GetItemCacheWithId(basket.Customer, ItemCacheType.Basket);
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

        public static void Save(IMerchelloContext merchelloContext, IBasket basket)
        {
            merchelloContext.Services.ItemCacheService.Save(((Basket)basket).ItemCache);
            Refresh(merchelloContext, basket);
        }

        /// <summary>
        /// Accepts visitor class to visit basket items
        /// </summary>
        /// <param name="vistor"><see cref="ILineItemVisitor"/></param>
        public void Accept(ILineItemVisitor vistor)
        {
            _itemCache.Items.Accept(vistor);
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
            get { return Items.Sum(x => (x.Amount * x.Quantity)); }
        }

        #endregion

        /// <summary>
        /// Generates a unique cache key for runtime caching of the basket
        /// </summary>
        /// <param name="customer"><see cref="ICustomerBase"/></param>
        /// <returns></returns>
        private static string MakeCacheKey(ICustomerBase customer)
        {
            return CachingBacker.CustomerBasketCacheKey(customer.Key, EnumTypeFieldConverter.CustomerItemItemCache.Basket.TypeKey);
        }

        
    }
}