﻿namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;

    using Merchello.Core.Models.DetachedContent;

    using Umbraco.Core;

    /// <summary>
    /// Defines a product variant
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Product : Entity, IProduct
    {
        /// <summary>
        /// The product options changed selector.
        /// </summary>
        private static readonly PropertyInfo ProductOptionsChangedSelector = ExpressionHelper.GetPropertyInfo<Product, ProductOptionCollection>(x => x.ProductOptions);

        /// <summary>
        /// The product variants changed selector.
        /// </summary>
        private static readonly PropertyInfo ProductVariantsChangedSelector = ExpressionHelper.GetPropertyInfo<Product, ProductVariantCollection>(x => x.ProductVariants);        

        /// <summary>
        /// The master product variant.
        /// </summary>
        private readonly IProductVariant _variant;

        /// <summary>
        /// A collection of product options associated with the product.
        /// </summary>
        private ProductOptionCollection _productOptions;

        /// <summary>
        /// A collection of product variants associated with the product.
        /// </summary>
        private ProductVariantCollection _productVariants;

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="variant">
        /// The variant.
        /// </param>
        public Product(IProductVariant variant)
            : this(variant, new ProductOptionCollection(), new ProductVariantCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="variant">
        /// The variant.
        /// </param>
        /// <param name="productOptions">
        /// The product options.
        /// </param>
        /// <param name="productVariants">
        /// The product variants.
        /// </param>
        public Product(
            IProductVariant variant,
            ProductOptionCollection productOptions,
            ProductVariantCollection productVariants)
        {
            Mandate.ParameterNotNull(variant, "variantMaster");
            Mandate.ParameterNotNull(productOptions, "optionCollection");
            Mandate.ParameterNotNull(productVariants, "productVariants");

            _variant = variant;
            _productOptions = productOptions;
            _productVariants = productVariants;
        }

        #region Overrides IProduct
        

        /// <summary>
        /// Gets a value indicating whether or not this group defines options
        /// </summary>
        [IgnoreDataMember]
        public bool DefinesOptions
        {
            get { return _productOptions.Any(); }            
        }

        /// <summary>
        /// Gets or sets the options that define the product attributes which 
        /// </summary>
        [DataMember]
        public ProductOptionCollection ProductOptions 
        {
            get
            {
                return _productOptions;
            }

            set 
            { 
                _productOptions = value;
                _productOptions.CollectionChanged += ProductOptionsChanged;
            }
        }

        /// <summary>
        /// Gets or sets the Product variants available for this product
        /// </summary>
        [DataMember]
        public ProductVariantCollection ProductVariants
        {
            get
            {
                return _productVariants;
            }

            set
            {
                _productVariants = value;
                _productVariants.CollectionChanged += ProductVariantsChanged;
            }
        }

        #endregion

        #region Overrides IProductBase

        /// <summary>
        /// Gets the product variant template's key
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey
        {
            get { return _variant.Key; }
            private set { _variant.Key = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _variant.Name; }
            set { _variant.Name = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's SKU
        /// </summary>
        [DataMember]
        public string Sku
        {
            get { return _variant.Sku; }
            set { _variant.Sku = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's price
        /// </summary>
        [DataMember]
        public decimal Price
        {
            get { return _variant.Price; }
            set { _variant.Price = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's cost of goods
        /// </summary>
        [DataMember]
        public decimal? CostOfGoods
        {
            get { return _variant.CostOfGoods; }
            set { _variant.CostOfGoods = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's sale price
        /// </summary>
        [DataMember]
        public decimal? SalePrice
        {
            get { return _variant.SalePrice; }
            set { _variant.SalePrice = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the product variant template's on sale value
        /// </summary>
        [DataMember]
        public bool OnSale
        {
            get { return _variant.OnSale; }
            set { _variant.OnSale = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's manufacturer
        /// </summary>
        [DataMember]
        public string Manufacturer
        {
            get { return _variant.Manufacturer; }
            set { _variant.Manufacturer = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's manufacturer
        /// </summary>
        [DataMember]
        public string ManufacturerModelNumber
        {
            get { return _variant.ManufacturerModelNumber; }
            set { _variant.ManufacturerModelNumber = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's weight
        /// </summary>
        [DataMember]
        public decimal? Weight
        {
            get { return _variant.Weight; }
            set { _variant.Weight = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's length
        /// </summary>
        [DataMember]
        public decimal? Length
        {
            get { return _variant.Length; }
            set { _variant.Length = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's width
        /// </summary>
        [DataMember]
        public decimal? Width
        {
            get { return _variant.Width; }
            set { _variant.Width = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's height
        /// </summary>
        [DataMember]
        public decimal? Height
        {
            get { return _variant.Height; }
            set { _variant.Height = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's barcode
        /// </summary>
        [DataMember]
        public string Barcode
        {
            get { return _variant.Barcode; }
            set { _variant.Barcode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the product variant template's available value is true
        /// </summary>
        [DataMember]
        public bool Available
        {
            get { return _variant.Available; }
            set { _variant.Available = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant template's tracks inventory value is true
        /// </summary>
        [DataMember]
        public bool TrackInventory
        {
            get { return _variant.TrackInventory; }
            set { _variant.TrackInventory = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant template's out of stock purchase value is true
        /// </summary>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get { return _variant.OutOfStockPurchase; }
            set { _variant.OutOfStockPurchase = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant template's taxable value is true
        /// </summary>
        [DataMember]
        public bool Taxable
        {
            get { return _variant.Taxable; }
            set { _variant.Taxable = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant template's shippable value is true
        /// </summary>
        [DataMember]
        public bool Shippable
        {
            get { return _variant.Shippable; }
            set { _variant.Shippable = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the product variant template's download value is true
        /// </summary>
        [DataMember]
        public bool Download
        {
            get { return _variant.Download; }
            set { _variant.Download = value; }
        }

        /// <summary>
        /// Gets or sets the product variant template's download media id value
        /// </summary>
        [DataMember]
        public int? DownloadMediaId
        {
            get { return _variant.DownloadMediaId; }
            set { _variant.DownloadMediaId = value; }
        }

        /// <summary>
        /// Gets the version key.
        /// </summary>
        [DataMember]
        public Guid VersionKey
        {
            get { return _variant.VersionKey; }
        }

        /// <summary>
        /// Gets the product variant template's inventory collection
        /// </summary>
        [DataMember]
        public IEnumerable<ICatalogInventory> CatalogInventories
        {
            get { return _variant.CatalogInventories; }
        }

        /// <summary>
        /// Gets the detached contents.
        /// </summary>
        [DataMember]
        public DetachedContentCollection<IProductVariantDetachedContent> DetachedContents
        {
            get
            {
                return _variant.DetachedContents;
            }
        }

        /// <summary>
        /// Gets the master variant.
        /// </summary>
        internal IProductVariant MasterVariant
        {
            get { return _variant; }
        }

        #endregion

        /// <summary>
        /// The reset dirty properties.
        /// </summary>
        public override void ResetDirtyProperties()
        {
            base.ResetDirtyProperties();
            _variant.ResetDirtyProperties();
        }

        /// <summary>
        /// The adding entity.
        /// </summary>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            ((ProductVariant)_variant).Master = true;
            ((ProductVariant)_variant).AddingEntity();
        }

        /// <summary>
        /// The updating entity.
        /// </summary>
        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            ((ProductVariant)_variant).UpdatingEntity();
        }

        /// <summary>
        /// The product options changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProductOptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ProductOptionsChangedSelector);
        }

        /// <summary>
        /// The product variants changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProductVariantsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ProductVariantsChangedSelector);
        }
    }
}