using System;
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
        private readonly ICustomerItemCacheService _customerItemCacheService;
        private readonly ICacheProvider _runtimeCache;
        private readonly Guid _customerKey;
        private readonly string _cacheKey;

        public Basket(ICustomerBase customer)
            : this(MerchelloContext.Current, customer.Key)
        {}

        public Basket(IMerchelloContext merchelloContext, Guid customerKey)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterCondition(!Guid.Empty.Equals(customerKey), "customerKey");

            _customerItemCacheService = merchelloContext.Services.CustomerItemCacheService;
            _runtimeCache = merchelloContext.Cache.RuntimeCache;
            _customerKey = customerKey;
            _cacheKey = CachingBacker.CustomerItemCacheKey(customerKey, EnumTypeFieldConverter.CustomerItemItemCache.Basket.TypeKey);
        }

        #region Overrides IBasket

        public void AddItem(Guid productVariantKey)
        {
            throw new NotImplementedException();
        }

        public void AddItem(Guid productVariantKey, ExtendedDataCollection extendedData)
        {
            throw new NotImplementedException();
        }

        public void AddItem(IProductVariant productVariant)
        {
            throw new NotImplementedException();
        }

        public void AddItem(IProductVariant productVariant, ExtendedDataCollection extendedData)
        {
            throw new NotImplementedException();
        }

        public void AddItem(string name, string sku, decimal price)
        {
            throw new NotImplementedException();
        }

        public void AddItem(string name, string sku, decimal price, ExtendedDataCollection extendedData)
        {
            throw new NotImplementedException();
        }

        public void AddItem(ILineItem lineItem)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuantity(int id, int quantity)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuantity(string sku, int quantity)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuantity(Guid productVariantKey, int quantity)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuantity(IProductVariant productVariant, int quantity)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string sku)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(Guid productVariantKey)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(IProductVariant productVariant)
        {
            throw new NotImplementedException();
        }

        public void Empty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        public void Refresh()
        {
            _runtimeCache.ClearCacheItem(_cacheKey);
            GetCustomerItemCache();
        }

        public LineItemCollection Items { get; private set; }

        #endregion


        private void GetCustomerItemCache()
        {
            var itemCache = (ICustomerItemCache)_runtimeCache.GetCacheItem(_cacheKey);
            if (itemCache == null)
            {
                //itemCache = _customerItemCacheService.GetCustomerItemCacheByCustomer(_customerKey, ItemCacheType.Basket)
            }
            _customerItemCache = itemCache;
        }

        
    }
}