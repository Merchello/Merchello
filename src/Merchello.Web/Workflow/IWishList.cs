namespace Merchello.Web.Workflow
{
    using System;
    using Core.Models;

    public interface IWishList
    {
        /// <summary>
        /// Gets the Wishlist version key
        /// </summary>
        Guid VersionKey { get; }

        /// <summary>
        /// Gets the customer associated with the basket
        /// </summary>
        ICustomerBase Customer { get; }

        /// <summary>
        /// Gets the basket line items
        /// </summary>
        LineItemCollection Items { get; }

        /// <summary>
        /// Gets the basket's item count
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Gets the sum of all basket item quantities
        /// </summary>
        int TotalQuantityCount { get; }

        /// <summary>
        /// Gets the sum of all basket item "amount" multiplied by quantity (price)
        /// </summary>
        decimal TotalWishListPrice { get; }

        /// <summary>
        /// Gets a value indicating whether or not the basket contains any items
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the basket
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the basket
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, int quantity);
        
        /// <summary>
        /// Adds a <see cref="IProduct"/> to the basket
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="name">Override for the name of the product in the line item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, string name, int quantity);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the basket
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
        /// Adds a <see cref="IProductVariant"/> to the basket
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        void AddItem(IProductVariant productVariant);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the basket
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        void AddItem(IProductVariant productVariant, int quantity);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the basket
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
        /// Adds a <see cref="IProductVariant"/> to the basket
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
        /// Adds a item to the basket
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sku">The SKU of the item</param>
        /// <param name="price">The price of the item</param>
        void AddItem(string name, string sku, decimal price);

        /// <summary>
        /// Adds a item to the basket
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sku">The SKU of the item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <param name="price">The price of the item</param>
        void AddItem(string name, string sku, int quantity, decimal price);

        /// <summary>
        /// Adds a item to the basket
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
        /// Updates the quantity of an item in the basket
        /// </summary>
        /// <param name="key">
        /// The line item key
        /// </param>
        /// <param name="quantity">
        /// The new quantity to be represented
        /// </param>
        void UpdateQuantity(Guid key, int quantity);

        /// <summary>
        /// Updates the quantity of an item in the basket
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        void UpdateQuantity(string sku, int quantity);

        /// <summary>
        /// Updates the quantity of an item in the basket
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        void UpdateQuantity(IProductVariant productVariant, int quantity);
      
        /// <summary>
        /// Removes an item from the basket  
        /// </summary>
        /// <param name="itemKey">The key of the line item to be removed</param>    
        void RemoveItem(Guid itemKey);     
        
        /// <summary>
        /// Removes an item from the basket
        /// </summary>
        /// <param name="sku">The SKU of the line item to be removed</param>
        void RemoveItem(string sku);        
        
        /// <summary>
        /// Removes a product variant from the basket
        /// </summary>
        /// <param name="productVariant">The product variant to be removed</param>
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
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> class</param>
        void Accept(ILineItemVisitor visitor);

        /// <summary>
        /// Moves the wishlist into a basket
        /// </summary>
        void MoveToBasket();
    }
}