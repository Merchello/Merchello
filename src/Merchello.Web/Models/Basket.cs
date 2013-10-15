using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using Merchello.Web.Cache;
using Umbraco.Core.Cache;

namespace Merchello.Web.Models
{
    public class Basket : IBasket
    {
        private readonly IItemCache _itemCache;
        private readonly ICustomerBase _customer;

        internal Basket(IItemCache itemCache, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(itemCache, "customerItemCache");
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

        #region Overrides IBasket

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(IProductVariant productVariant)
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.AddProductVariantValues(productVariant);
            
            AddItem(productVariant.Name, productVariant.Sku, 
                productVariant.OnSale ? 
                productVariant.SalePrice != null ? productVariant.SalePrice.Value : productVariant.Price
                : productVariant.Price, extendedData);
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(string name, string sku, decimal price)
        {
            AddItem(name, sku, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(string name, string sku, decimal price, ExtendedDataCollection extendedData)
        {
            _itemCache.AddItem(LineItemType.Product, name, sku, 1, price, extendedData);
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

        #endregion

        private static string MakeCacheKey(ICustomerBase customer)
        {
            return CachingBacker.CustomerBasketCacheKey(customer.Key, EnumTypeFieldConverter.CustomerItemItemCache.Basket.TypeKey);
        }

        
    }
}