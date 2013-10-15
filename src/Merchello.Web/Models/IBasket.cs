using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models
{
    public interface IBasket
    {
        // Adds an item to the basket
        void AddItem(IProductVariant productVariant);
        void AddItem(string name, string sku, decimal price);
        void AddItem(string name, string sku, decimal price, ExtendedDataCollection extendedData);
        void AddItem(ICustomerItemCacheLineItem lineItem);

        // Updates the quantity of an item in the basket
        void UpdateQuantity(int id, int quantity);
        void UpdateQuantity(string sku, int quantity);
        void UpdateQuantity(IProductVariant productVariant, int quantity);
      
        /// Removes an item from the basket      
        void RemoveItem(int id);     
        void RemoveItem(string sku);        
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
        /// Saves the basket
        /// </summary>
        void Save();

        /// <summary>
        /// Accepts visitor class to visit basket items
        /// </summary>
        /// <param name="vistor"><see cref="ILineItemVisitor"/></param>
        void Accept(ILineItemVisitor vistor);

        /// <summary>
        /// The customer assoicated with the basket
        /// </summary>
        ICustomerBase Customer { get; }

        /// <summary>
        /// The basket line items
        /// </summary>
        LineItemCollection Items { get; }
    }
}