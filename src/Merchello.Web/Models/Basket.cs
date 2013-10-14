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
        private ICustomerItemCache _customerItemCache;
        private readonly IServiceContext _serviceContext;
        private readonly ICacheProvider _runtimeCache;
        private readonly ICustomerBase _customer;
        private readonly string _cacheKey;

        public Basket(ICustomerBase customer)
            : this(MerchelloContext.Current, customer)
        {}

        public Basket(IMerchelloContext merchelloContext, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(customer, "customer");

            _serviceContext = merchelloContext.Services;
            _runtimeCache = merchelloContext.Cache.RuntimeCache;
            _customer = customer;
            _cacheKey = CachingBacker.CustomerItemCacheKey(customer.Key, EnumTypeFieldConverter.CustomerItemItemCache.Basket.TypeKey);
        }

        #region Overrides IBasket

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(Guid productVariantKey)
        {
            var variant = _serviceContext.ProductVariantService.GetByKey(productVariantKey);
            AddItem(variant);
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(IProductVariant productVariant)
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.SetValue("MerchProductKey", productVariant.ProductKey.ToString());
            extendedData.SetValue("MerchProductVariantKey", productVariant.Key.ToString());
            extendedData.SetValue("MerchCostOfGoods", productVariant.CostOfGoods.ToString());
            extendedData.SetValue("MerchWeight", productVariant.Weight.ToString());
            extendedData.SetValue("MerchWidth", productVariant.Width.ToString());
            extendedData.SetValue("MerchHeight", productVariant.Height.ToString());
            extendedData.SetValue("MerchBarcode", productVariant.Barcode);
            extendedData.SetValue("MerchTrackInventory", productVariant.TrackInventory.ToString());
            extendedData.SetValue("MerchOutOfStockPurchase", productVariant.OutOfStockPurchase.ToString());
            extendedData.SetValue("MerchTaxable", productVariant.Taxable.ToString());
            extendedData.SetValue("MerchShippable", productVariant.Shippable.ToString());
            extendedData.SetValue("MerchDownload", productVariant.Download.ToString());
            extendedData.SetValue("MerchDownloadMediaId", productVariant.DownloadMediaId.ToString());
            
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
            AddItem(new CustomerItemCacheLineItem(_customerItemCache.Id, LineItemType.Product, name, sku, 1, price, extendedData));
        }

        /// <summary>
        /// Adds a line item to the basket
        /// </summary>
        public void AddItem(ICustomerItemCacheLineItem lineItem)
        {
            Mandate.ParameterNotNull(lineItem, "lineItem");
            _customerItemCache.Items.Add(lineItem);
        }

        /// <summary>
        /// Updates a basket item's quantity
        /// </summary>
        public void UpdateQuantity(Guid productVariantKey, int quantity)
        {
            var variant = _serviceContext.ProductVariantService.GetByKey(productVariantKey);
            if (variant != null) UpdateQuantity(variant, quantity);
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
            var item = _customerItemCache.Items.FirstOrDefault(x => x.Id == id);
            if(item != null) UpdateQuantity(item.Sku, quantity);
        }

        /// <summary>
        /// Updates a basket item's quantity
        /// </summary>
        public void UpdateQuantity(string sku, int quantity)
        {           
            if (!_customerItemCache.Items.Contains(sku)) return;
            if (quantity <= 0)
            {
                RemoveItem(sku);
                return;
            }
            _customerItemCache.Items[sku].Quantity = quantity;
        }

        /// <summary>
        /// Removes a basket line item
        /// </summary>
        public void RemoveItem(int id)
        {
            var item = _customerItemCache.Items.FirstOrDefault(x => x.Id == id);
            if(item != null) RemoveItem(item.Sku);
        }        

        /// <summary>
        /// Removes a basket line item
        /// </summary>
        public void RemoveItem(Guid productVariantKey)
        {
            var variant = _serviceContext.ProductVariantService.GetByKey(productVariantKey);
            if(variant != null) RemoveItem(variant.Sku);
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
            if(_customerItemCache.Items.Contains(sku)) _customerItemCache.Items.RemoveItem(sku);
        }

        /// <summary>
        /// Empties the basket
        /// </summary>
        public void Empty()
        {
            _customerItemCache.Items.Clear();
        }

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        public void Refresh()
        {
            _runtimeCache.ClearCacheItem(_cacheKey);
            GetCustomerItemCache();
        }

        /// <summary>
        /// Saves the basket
        /// </summary>
        public void Save()
        {
            _serviceContext.CustomerItemCacheService.Save(_customerItemCache);
            Refresh();
        }

        /// <summary>
        /// Accepts visitor class to visit basket items
        /// </summary>
        /// <param name="vistor"><see cref="ILineItemVisitor"/></param>
        public void Accept(ILineItemVisitor vistor)
        {
            _customerItemCache.Items.Accept(vistor);
        }

        public LineItemCollection Items
        {
            get { return _customerItemCache.Items; }
        }

        #endregion


        private void GetCustomerItemCache()
        {
            var itemCache = (ICustomerItemCache)_runtimeCache.GetCacheItem(_cacheKey);
            if (itemCache == null)
            {
                itemCache = _serviceContext.CustomerItemCacheService.GetCustomerItemCacheWithId(_customer, ItemCacheType.Basket);
                _runtimeCache.GetCacheItem(_cacheKey, () => itemCache);
            }
            _customerItemCache = itemCache;
        }

        
    }
}