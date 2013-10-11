using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models
{
    public interface IBasket
    {
        // Adds an item to the basket
        void AddItem(Guid productVariantKey);
        void AddItem(Guid productVariantKey, ExtendedDataCollection extendedData);
        void AddItem(IProductVariant productVariant);
        void AddItem(IProductVariant productVariant, ExtendedDataCollection extendedData);
        void AddItem(string name, string sku, decimal price);
        void AddItem(string name, string sku, decimal price, ExtendedDataCollection extendedData);
        void AddItem(ILineItem lineItem);

        // Updates the quantity of an item in the basket
        void UpdateQuantity(int id, int quantity);
        void UpdateQuantity(string sku, int quantity);
        void UpdateQuantity(Guid productVariantKey, int quantity);
        void UpdateQuantity(IProductVariant productVariant, int quantity);
      
        /// Removes an item from the basket      
        void RemoveItem(int id);     
        void RemoveItem(string sku);        
        void RemoveItem(Guid productVariantKey);        
        void RemoveItem(IProductVariant productVariant);

        /// <summary>
        /// Empties the basket
        /// </summary>
        void Empty();

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        void Refresh();

        /// <summary>
        /// The basket line items
        /// </summary>
        LineItemCollection Items { get; }
    }
}