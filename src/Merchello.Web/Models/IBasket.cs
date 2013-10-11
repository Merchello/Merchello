using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models
{
    public interface IBasket
    {
        void AddItem(Guid productVariantKey);
        void AddItem(Guid productVariantKey, ExtendedDataCollection extendedData);
        void AddItem(IProductVariant productVariant);
        void AddItem(IProductVariant productVariant, ExtendedDataCollection extendedData);
        void AddItem(string name, string sku, decimal price);
        void AddItem(string name, string sku, decimal price, ExtendedDataCollection extendedData);
        void AddItem(ILineItem lineItem);

        void UpdateQuantity(int id, int quantity);
        void UpdateQuantity(string sku, int quantity);
        void UpdateQuantity(Guid productVariantKey, int quantity);
        void UpdateQuantity(IProductVariant productVariant, int quantity);

        void RemoveItem(int id);
        void RemoveItem(string sku);
        void RemoveItem(Guid productVariantKey);
        void RemoveItem(IProductVariant productVariant);

        void Clear();

        LineItemCollection Items { get; }
    }
}