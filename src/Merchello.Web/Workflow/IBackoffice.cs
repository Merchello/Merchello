using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Web.Workflow
{
    /// <summary>
    /// The Backoffice interface
    /// </summary>
    public interface IBackoffice
    {
        /// <summary>
        /// Gets the backoffices version key
        /// </summary>
        Guid VersionKey { get; }

        /// <summary>
        /// Gets the customer associated with the backoffice
        /// </summary>
        ICustomerBase Customer { get; }

        /// <summary>
        /// Gets the backoffice line items
        /// </summary>
        LineItemCollection Items { get; }

        /// <summary>
        /// Gets the backoffice's item count
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Gets the sum of all backoffice item quantities
        /// </summary>
        int TotalQuantityCount { get; }

        /// <summary>
        /// Gets the sum of all backoffice item "amount" multiplied by quantity (price)
        /// </summary>
        decimal TotalBackofficePrice { get; }

        /// <summary>
        /// Gets a value indicating whether or not the backoffice contains any items
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the backoffice
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the backoffice
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, int quantity);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the backoffice
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="name">Override for the name of the product in the line item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, string name, int quantity);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the backoffice
        /// </summary>
        /// <param name="product">
        /// The <see cref="IProduct"/> to be added
        /// </param>
        /// <param name="name">
        /// Override for the name of the product in the line item
        /// </param>
        /// <param name="quantity">
        /// The quantity to be represented
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, string name, int quantity, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the backoffice
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        void AddItem(IProductVariant productVariant);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the backoffice
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        void AddItem(IProductVariant productVariant, int quantity);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product variant to be added
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="quantity">
        /// The quantity to be represented
        /// </param>
        void AddItem(IProductVariant productVariant, string name, int quantity);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product variant to be added
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="quantity">
        /// The quantity to be represented
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        void AddItem(IProductVariant productVariant, string name, int quantity, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a item to the backoffice
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sku">The SKU of the item</param>
        /// <param name="price">The price of the item</param>
        void AddItem(string name, string sku, decimal price);

        /// <summary>
        /// Adds a item to the backoffice
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sku">The SKU of the item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <param name="price">The price of the item</param>
        void AddItem(string name, string sku, int quantity, decimal price);

        /// <summary>
        /// Adds a item to the backoffice
        /// </summary>
        /// <param name="name">
        /// The name of the item
        /// </param>
        /// <param name="sku">
        /// The SKU of the item
        /// </param>
        /// <param name="quantity">
        /// The quantity to be represented
        /// </param>
        /// <param name="price">
        /// The price of the item
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        void AddItem(string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData);

        /// <summary>
        /// Updates the quantity of an item in the backoffice
        /// </summary>
        /// <param name="key">
        /// The line item key
        /// </param>
        /// <param name="quantity">
        /// The new quantity to be represented
        /// </param>
        void UpdateQuantity(Guid key, int quantity);

        /// <summary>
        /// Updates the quantity of an item in the backoffice
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        void UpdateQuantity(string sku, int quantity);

        /// <summary>
        /// Updates the quantity of an item in the backoffice
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        void UpdateQuantity(IProductVariant productVariant, int quantity);

        /// <summary>
        /// Removes an item from the backoffice  
        /// </summary>
        /// <param name="itemKey">The key of the line item to be removed</param>    
        void RemoveItem(Guid itemKey);

        /// <summary>
        /// Removes an item from the backoffice
        /// </summary>
        /// <param name="sku">The SKU of the line item to be removed</param>
        void RemoveItem(string sku);

        /// <summary>
        /// Removes a product variant from the backoffice
        /// </summary>
        /// <param name="productVariant">The product variant to be removed</param>
        void RemoveItem(IProductVariant productVariant);

        /// <summary>
        /// Empties the backoffice
        /// </summary>
        void Empty();

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        void Refresh();

        /// <summary>
        /// Saves the backoffice
        /// </summary>
        void Save();

        /// <summary>
        /// Accepts visitor class to visit backoffice items
        /// </summary>
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> class</param>
        void Accept(ILineItemVisitor visitor);       
    }
}
