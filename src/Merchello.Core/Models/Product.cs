namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;

    using Merchello.Core.Models.DetachedContent;

    /// <summary>
    /// Defines a product variant
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Product : Entity, IProduct
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

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
            Ensure.ParameterNotNull(variant, "variantMaster");
            Ensure.ParameterNotNull(productOptions, "optionCollection");
            Ensure.ParameterNotNull(productVariants, "productVariants");

            _variant = variant;
            _productOptions = productOptions;
            _productVariants = productVariants;
        }

        #region Overrides IProduct
        

        /// <inheritdoc/>
        [IgnoreDataMember]
        public bool DefinesOptions
        {
            get
            {
                return _productOptions.Any();
            }            
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        [DataMember]
        public Guid ProductVariantKey
        {
            get
            {
                return _variant.Key;
            }

            private set
            {
                _variant.Key = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _variant.Name;
            }

            set
            {
                _variant.Name = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Sku
        {
            get
            {
                return _variant.Sku;
            }

            set
            {
                _variant.Sku = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal Price
        {
            get
            {
                return _variant.Price;
            }

            set
            {
                _variant.Price = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? CostOfGoods
        {
            get
            {
                return _variant.CostOfGoods;
            }

            set
            {
                _variant.CostOfGoods = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? SalePrice
        {
            get
            {
                return _variant.SalePrice;
            }

            set
            {
                _variant.SalePrice = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool OnSale
        {
            get
            {
                return _variant.OnSale;
            }

            set
            {
                _variant.OnSale = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Manufacturer
        {
            get
            {
                return _variant.Manufacturer;
            }

            set
            {
                _variant.Manufacturer = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ManufacturerModelNumber
        {
            get
            {
                return _variant.ManufacturerModelNumber;
            }

            set
            {
                _variant.ManufacturerModelNumber = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Weight
        {
            get
            {
                return _variant.Weight;
            }

            set
            {
                _variant.Weight = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Length
        {
            get
            {
                return _variant.Length;
            }

            set
            {
                _variant.Length = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Width
        {
            get
            {
                return _variant.Width;
            }

            set
            {
                _variant.Width = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Height
        {
            get
            {
                return _variant.Height;
            }

            set
            {
                _variant.Height = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Barcode
        {
            get
            {
                return _variant.Barcode;
            }

            set
            {
                _variant.Barcode = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Available
        {
            get
            {
                return _variant.Available;
            }

            set
            {
                _variant.Available = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool TrackInventory
        {
            get
            {
                return _variant.TrackInventory;
            }

            set
            {
                _variant.TrackInventory = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get
            {
                return _variant.OutOfStockPurchase;
            }

            set
            {
                _variant.OutOfStockPurchase = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Taxable
        {
            get
            {
                return _variant.Taxable;
            }

            set
            {
                _variant.Taxable = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Shippable
        {
            get
            {
                return _variant.Shippable;
            }

            set
            {
                _variant.Shippable = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Download
        {
            get
            {
                return _variant.Download;
            }

            set
            {
                _variant.Download = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int? DownloadMediaId
        {
            get
            {
                return _variant.DownloadMediaId;
            }

            set
            {
                _variant.DownloadMediaId = value;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid VersionKey
        {
            get
            {
                return _variant.VersionKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public IEnumerable<ICatalogInventory> CatalogInventories
        {
            get
            {
                return _variant.CatalogInventories;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public DetachedContentCollection<IProductVariantDetachedContent> DetachedContents
        {
            get
            {
                return _variant.DetachedContents;
            }
        }

        /// <inheritdoc/>
        internal IProductVariant MasterVariant
        {
            get
            {
                return _variant;
            }
        }

        #endregion

        /// <inheritdoc/>
        public override void ResetDirtyProperties()
        {
            base.ResetDirtyProperties();
            _variant.ResetDirtyProperties();
        }

        /// <inheritdoc/>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            ((ProductVariant)_variant).Master = true;
            ((ProductVariant)_variant).AddingEntity();
        }

        /// <inheritdoc/>
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
            OnPropertyChanged(_ps.Value.ProductOptionsChangedSelector);
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
            OnPropertyChanged(_ps.Value.ProductVariantsChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The product options changed selector.
            /// </summary>
            public readonly PropertyInfo ProductOptionsChangedSelector = ExpressionHelper.GetPropertyInfo<Product, ProductOptionCollection>(x => x.ProductOptions);

            /// <summary>
            /// The product variants changed selector.
            /// </summary>
            public readonly PropertyInfo ProductVariantsChangedSelector = ExpressionHelper.GetPropertyInfo<Product, ProductVariantCollection>(x => x.ProductVariants);
        }
    }
}