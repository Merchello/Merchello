namespace Merchello.Web.Workflow.CustomerItemCache
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains;
    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Web.DataModifiers;
    using Merchello.Web.Models.ContentEditing;

    using umbraco.cms.businesslogic.language;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// A base class for customer Item caches.
    /// </summary>
    public abstract class CustomerItemCacheBase : ICustomerItemCacheBase
    {
        /// <summary>
        /// The product tax extended data keys.
        /// </summary>
        private static readonly string[] ProductTaxExtendedDataKeys = 
            {
                    Core.Constants.ExtendedDataKeys.ProductPriceTaxAmount,
                    Core.Constants.ExtendedDataKeys.ProductPriceNoTax,
                    Core.Constants.ExtendedDataKeys.ProductSalePriceNoTax,
                    Core.Constants.ExtendedDataKeys.ProductSalePriceTaxAmount
            };

        /// <summary>
        /// The item cache responsible for persisting the customer item cache contents.
        /// </summary>
        private readonly IItemCache _itemCache;

        /// <summary>
        /// The customer.
        /// </summary>
        private readonly ICustomerBase _customer;

        /// <summary>
        /// The product data modifier.
        /// </summary>
        private Lazy<IDataModifierChain<IProductVariantDataModifierData>> _productDataModifier; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerItemCacheBase"/> class.
        /// </summary>
        /// <param name="itemCache">
        /// The item cache.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        protected CustomerItemCacheBase(IItemCache itemCache, ICustomerBase customer)
        {
            Mandate.ParameterNotNull(itemCache, "ItemCache");
            Mandate.ParameterNotNull(customer, "customer");
            _customer = customer;
            _itemCache = itemCache;
            EnableDataModifiers = true;

            this.Initialize();
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable data modifiers.
        /// </summary>
        public bool EnableDataModifiers { get; set; }

        /// <summary>
        /// Gets the version of the customer item cache
        /// </summary>
        public Guid VersionKey
        {
            get { return _itemCache.VersionKey; }
            internal set { _itemCache.VersionKey = value; }
        }

        /// <summary>
        /// Gets the customer associated with the customer item cache
        /// </summary>
        public ICustomerBase Customer
        {
            get { return _customer; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public LineItemCollection Items
        {
            get { return _itemCache.Items; }
        }

        /// <summary>
        /// Gets the customer item cache's item count
        /// </summary>
        public int TotalItemCount
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Gets the sum of all customer item cache item quantities
        /// </summary>
        public int TotalQuantityCount
        {
            get { return Items.Sum(x => x.Quantity); }
        }

        /// <summary>
        /// Gets the sum of all customer item cache item "amount" (price)
        /// </summary>
        public decimal TotalItemCachePrice
        {
            get
            {
                var charges = Items.Where(x => x.LineItemType != LineItemType.Discount).Sum(x => (x.Quantity * x.Price));
                var discounts = Items.Where(x => x.LineItemType == LineItemType.Discount).Sum(x => (x.Quantity * x.Price));
                return charges - discounts;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the customer item cache contains any items
        /// </summary>
        public bool IsEmpty
        {
            get { return !Items.Any(); }
        }

        /// <summary>
        /// Gets the <see cref="IItemCache"/>
        /// </summary>
        internal IItemCache ItemCache
        {
            get { return _itemCache; }
        }

        #region IProduct

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the customer item cache item collection
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added to the customer item cache</param>
        public void AddItem(IProduct product)
        {
            AddItem(product, product.Name, 1);
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the customer item cache item collection
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void AddItem(IProduct product, int quantity)
        {
            AddItem(product, product.Name, quantity);
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the customer item cache item collection
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="name">The name to be used in the line item</param>
        /// <param name="quantity">The quantity to be added</param>
        public void AddItem(IProduct product, string name, int quantity)
        {
            AddItem(product, name, quantity, new ExtendedDataCollection());
        }

        /// <summary>
        /// Intended to be used by a <see cref="IProduct"/>s without options.  If the product does have options and a collection of <see cref="IProductVariant"/>s, the first
        /// <see cref="IProductVariant"/> is added to the customer item cache item collection
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to be added</param>
        /// <param name="name">The name of the product to be used in the line item</param>
        /// <param name="quantity">The quantity of the line item</param>
        /// <param name="extendedData">A <see cref="ExtendedDataCollection"/></param>
        public void AddItem(IProduct product, string name, int quantity, ExtendedDataCollection extendedData)
        {
            var variant = product.GetProductVariantForPurchase();
            if (variant != null)
            {
                AddItem(variant, name, quantity, extendedData);
                return;
            }

            if (!product.ProductVariants.Any()) return;

            AddItem(product.ProductVariants.First(), name, quantity, extendedData);
        }       

        #endregion

        #region ProductDisplay

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="ProductDisplay"/> to be added</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        public void AddItem(ProductDisplay product)
        {
            AddItem(product, 1);
        }

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="ProductDisplay"/> to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        public void AddItem(ProductDisplay product, int quantity)
        {
            AddItem(product, product.Name, quantity);
        }

        /// <summary>
        /// Adds a <see cref="ProductDisplay"/> to the item cache
        /// </summary>
        /// <param name="product">The <see cref="ProductDisplay"/> to be added</param>
        /// <param name="name">Override for the name of the product in the line item</param>
        /// <param name="quantity">The quantity to be represented</param>
        /// <remarks>
        /// If the product has variants, the "first" variant found will be added.
        /// </remarks>
        public void AddItem(ProductDisplay product, string name, int quantity)
        {
            AddItem(product, name, quantity, new ExtendedDataCollection());
        }

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
        public void AddItem(ProductDisplay product, string name, int quantity, ExtendedDataCollection extendedData)
        {
            var variant = product.ProductVariants.Any() ?
                        product.ProductVariants.First() : 
                        product.AsMasterVariantDisplay();
            AddItem(variant, name == product.Name ? variant.Name : name, quantity, extendedData);
        }

        #endregion


        #region IProductVariant

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        public void AddItem(IProductVariant productVariant)
        {
            AddItem(productVariant, productVariant.Name, 1);
        }

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void AddItem(IProductVariant productVariant, int quantity)
        {
            AddItem(productVariant, productVariant.Name, quantity);
        }

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void AddItem(IProductVariant productVariant, string name, int quantity)
        {
            AddItem(productVariant, name, quantity, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        public void AddItem(IProductVariant productVariant, string name, int quantity, ExtendedDataCollection extendedData)
        {
            AddItem(productVariant.ToProductVariantDisplay(), name, quantity, extendedData);
        }

        #endregion

        #region ProductVariantDisplay

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        public void AddItem(ProductVariantDisplay productVariant)
        {
            AddItem(productVariant, 1);
        }

        /// <summary>
        /// Adds a <see cref="ProductVariantDisplay"/> to the item cache
        /// </summary>
        /// <param name="productVariant">The product variant to be added</param>
        /// <param name="quantity">The quantity to be represented</param>
        public void AddItem(ProductVariantDisplay productVariant, int quantity)
        {
            AddItem(productVariant, productVariant.Name, quantity);
        }

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
        public void AddItem(ProductVariantDisplay productVariant, string name, int quantity)
        {
            AddItem(productVariant, name, quantity, new ExtendedDataCollection());
        }

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
        public void AddItem(ProductVariantDisplay productVariant, string name, int quantity, ExtendedDataCollection extendedData)
        {
            if (EnableDataModifiers)
            {
                var attempt = _productDataModifier.Value.Modify(productVariant);
                if (attempt.Success)
                {
                    var modified = attempt.Result as ProductVariantDisplay;
                    if (modified != null)
                    {
                        extendedData.MergeDataModifierLogs(modified);
                        if (!extendedData.DefinesProductVariant()) extendedData.AddProductVariantValues(modified);
                        productVariant = modified;
                    }
                }
            }
            else
            {
                extendedData.MergeDataModifierLogs(productVariant);
                if (!extendedData.DefinesProductVariant()) extendedData.AddProductVariantValues(productVariant);
            }
            
            var price = productVariant.OnSale ? extendedData.GetSalePriceValue() : extendedData.GetPriceValue();


            AddItem(string.IsNullOrEmpty(name) ? productVariant.Name : name, productVariant.Sku, quantity, price, extendedData);
        }

        #endregion

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        public void AddItem(string name, string sku, decimal price)
        {
            AddItem(name, sku, 1, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        public void AddItem(string name, string sku, int quantity, decimal price)
        {
            AddItem(name, sku, quantity, price, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the customer item cache
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        public void AddItem(string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData)
        {
            if (quantity <= 0) quantity = 1;
            if (price < 0) price = 0;
            var lineItem = new ItemCacheLineItem(LineItemType.Product, name, sku, quantity, price, extendedData);                        
            _itemCache.AddItem(lineItem);
        }

        /// <summary>
        /// Adds a line item to the customer item cache.
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="IItemCacheLineItem"/>.
        /// </param>
        public void AddItem(IItemCacheLineItem lineItem)
        {
            if (lineItem.Quantity <= 0) lineItem.Quantity = 1;
            if (lineItem.Price < 0) lineItem.Price = 0;            
            _itemCache.AddItem(lineItem);
        }

        /// <summary>
        /// Updates a customer item cache item's quantity
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void UpdateQuantity(IProductVariant productVariant, int quantity)
        {
            UpdateQuantity(productVariant.Sku, quantity);
        }

        /// <summary>
        /// Updates a customer item cache item's quantity
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public void UpdateQuantity(Guid key, int quantity)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Key == key);

            if (item != null) UpdateQuantity(item.Sku, quantity);
        }

        /// <summary>
        /// Updates a customer item cache item's quantity
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
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
        /// Removes a customer item cache line item
        /// </summary>
        /// <param name="itemKey">
        /// The item Key.
        /// </param>
        public void RemoveItem(Guid itemKey)
        {
            var item = _itemCache.Items.FirstOrDefault(x => x.Key == itemKey);

            if (item != null) RemoveItem(item.Sku);
        }

        /// <summary>
        /// Removes a customer item cache line item
        /// </summary>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        public void RemoveItem(IProductVariant productVariant)
        {
            RemoveItem(productVariant.Sku);
        }

        /// <summary>
        /// Removes a customer item cache line item
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        public void RemoveItem(string sku)
        {
            LogHelper.Debug<CustomerItemCacheBase>("Before Attempting to remove - count: " + _itemCache.Items.Count);
            LogHelper.Debug<CustomerItemCacheBase>("Attempting to remove sku: " + sku);
            _itemCache.Items.RemoveItem(sku);
            LogHelper.Debug<CustomerItemCacheBase>("After Attempting to remove - count: " + _itemCache.Items.Count);
        }

        /// <summary>
        /// Empties the customer item cache
        /// </summary>
        public abstract void Empty();


        /// <summary>
        /// Refreshes cache with database values
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// Saves the customer item cache
        /// </summary>
        public abstract void Save();
        
        /// <summary>
        /// Accepts visitor class to visit customer item cache items
        /// </summary>
        /// <param name="visitor">The <see cref="ILineItemVisitor"/> to walk the collection</param>
        public void Accept(ILineItemVisitor visitor)
        {
            _itemCache.Items.Accept(visitor);
        }

        /// <summary>
        /// Handles the adding item event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="AddItemEventArgs"/>.
        /// </param>        
        private static void ItemsOnAddingItem(object sender, AddItemEventArgs e)
        {
            var item = e.LineItem;
            if (item.ExtendedData.ContainsAny(ProductTaxExtendedDataKeys))
            {
                item.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.TaxIncludedInProductPrice, true.ToString());
            }
        }

        /// <summary>
        /// Initializes the Lazy data modifiers
        /// </summary>
        private void Initialize()
        {
            _productDataModifier = new Lazy<IDataModifierChain<IProductVariantDataModifierData>>(() => new ProductVariantDataModifierChain(MerchelloContext.Current));
            _itemCache.Items.AddingItem += ItemsOnAddingItem;
        }

    }
}