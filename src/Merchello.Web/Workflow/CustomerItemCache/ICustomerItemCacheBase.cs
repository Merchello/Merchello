namespace Merchello.Web.Workflow.CustomerItemCache
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The CustomerItemCacheBase interface.
    /// </summary>
    public interface ICustomerItemCacheBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether enable data modifiers.
        /// </summary>
        bool EnableDataModifiers { get; set; }

        /// <summary>
        /// Gets the item caches version key
        /// </summary>
        Guid VersionKey { get; }

        /// <summary>
        /// Gets the customer associated with the item cache
        /// </summary>
        ICustomerBase Customer { get; }

        /// <summary>
        /// Gets the item cache line items
        /// </summary>
        LineItemCollection Items { get; }

        /// <summary>
        /// Gets the item cache's item count
        /// </summary>
        int TotalItemCount { get; }

        /// <summary>
        /// Gets the sum of all item cache item quantities
        /// </summary>
        int TotalQuantityCount { get; }

        /// <summary>
        /// Gets the sum of all item cache item "amount" multiplied by quantity (price)
        /// </summary>
        decimal TotalItemCachePrice { get; }

        /// <summary>
        /// Gets a value indicating whether or not the item cache contains any items
        /// </summary>
        bool IsEmpty { get; }

        #region IProduct

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/>.</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, int quantity);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/>.</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, int quantity, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="name">Override for the name of the product in the line item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(IProduct product, string name, int quantity);

        /// <summary>
        /// Adds a <see cref="IProduct"/> to the item cache
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

        #endregion

        #region ProductDisplay

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="ProductDisplay"/> to be added</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(ProductDisplay product);

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="ProductDisplay"/> to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(ProductDisplay product, int quantity);

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="ProductDisplay"/> to be added</param>
        /// <param name="name">Override for the name of the product in the line item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        void AddItem(ProductDisplay product, string name, int quantity);

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">
        /// The <see cref="ProductDisplay"/> to be added
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
        void AddItem(ProductDisplay product, string name, int quantity, ExtendedDataCollection extendedData);

        #endregion


        #region IProductVariant

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        void AddItem(IProductVariant productVariant);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/>.</param>
        void AddItem(IProductVariant productVariant, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        void AddItem(IProductVariant productVariant, int quantity);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/>.</param>
        void AddItem(IProductVariant productVariant, int quantity, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> to the item cache
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
        /// Adds a <see cref="IProductVariant"/> to the item cache
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

        #endregion

        #region ProductVariantDisplay

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        void AddItem(ProductVariantDisplay productVariant);

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/>.</param>
        void AddItem(ProductVariantDisplay productVariant, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        void AddItem(ProductVariantDisplay productVariant, int quantity);

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/>.</param>
        void AddItem(ProductVariantDisplay productVariant, int quantity, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
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
        void AddItem(ProductVariantDisplay productVariant, string name, int quantity);

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
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
        void AddItem(ProductVariantDisplay productVariant, string name, int quantity, ExtendedDataCollection extendedData);

        #endregion


        /// <summary>
        /// Adds a item to the item cache
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sku">The SKU of the item</param>
        /// <param name="price">The price of the item</param>
        void AddItem(string name, string sku, decimal price);

        /// <summary>
        /// Adds a item to the item cache
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sku">The SKU of the item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <param name="price">The price of the item</param>
        void AddItem(string name, string sku, int quantity, decimal price);

        /// <summary>
        /// 
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
        /// </param>Adds a item to the item cache
        void AddItem(string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData);

        /// <summary>
        /// Adds a line item to the item cache.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="IItemCacheLineItem"/>.
        /// </param>
        void AddItem(IItemCacheLineItem lineItem);

        /// <summary>
        /// Updates the quantity of an item in the item cache
        /// </summary>
        /// <param name="key">
        /// The line item key
        /// </param>
        /// <param name="quantity">
        /// The new quantity to be represented
        /// </param>
        void UpdateQuantity(Guid key, int quantity);

        /// <summary>
        /// Updates the quantity of an item in the item cache
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        void UpdateQuantity(string sku, int quantity);

        /// <summary>
        /// Update price
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="price"></param>
        void UpdatePrice(string sku, decimal price);

        /// <summary>
        /// Updates the quantity of an item in the item cache
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        void UpdateQuantity(IProductVariant productVariant, int quantity);

        /// <summary>
        /// Removes an item from the item cache  
        /// </summary>
        /// <param name="itemKey">The key of the line item to be removed</param>    
        void RemoveItem(Guid itemKey);

        /// <summary>
        /// Removes an item from the item cache
        /// </summary>
        /// <param name="sku">The SKU of the line item to be removed</param>
        void RemoveItem(string sku);

        /// <summary>
        /// Removes a product variant from the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be removed</param>
        void RemoveItem(IProductVariant productVariant);

        /// <summary>
        /// Empties the item cache
        /// </summary>
        void Empty();

        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        void Refresh();

        /// <summary>
        /// Saves the item cache
        /// </summary>
        void Save();

        /// <summary>
        /// Validates values stored in the internal item cache to make certain items being purchase
        /// reflect most recent values in the back office.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Validate();

        /// <summary>
        /// Accepts visitor class to visit item cache items
        /// </summary>
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> class</param>
        void Accept(ILineItemVisitor visitor);  
    }
}