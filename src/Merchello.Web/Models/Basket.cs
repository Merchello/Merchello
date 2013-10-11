using System;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models
{
    public class Basket : IBasket
    {
        private ICustomerItemCache _customerItemCache;

        public Basket(ICustomerBase customer)
            : this(customer.Key)
        {}

        public Basket(Guid customerKey)
        {
            
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

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public LineItemCollection Items { get; private set; }

        #endregion
    }
}