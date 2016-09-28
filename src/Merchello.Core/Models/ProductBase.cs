namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;

    using Merchello.Core.Models.DetachedContent;

    /// <summary>
    /// Represents an abstract class for base Product properties and methods
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class ProductBase : Entity, IProductBase
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The SKU.
        /// </summary>
        private string _sku;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The price.
        /// </summary>
        private decimal _price;

        /// <summary>
        /// The cost of goods.
        /// </summary>
        private decimal? _costOfGoods;

        /// <summary>
        /// The on sale.
        /// </summary>
        private bool _onSale;

        /// <summary>
        /// The sale price.
        /// </summary>
        private decimal? _salePrice;

        /// <summary>
        /// The manufacturer.
        /// </summary>
        private string _manufacturer;

        /// <summary>
        /// The manufacturer model number.
        /// </summary>
        private string _manufacturerModelNumber;

        /// <summary>
        /// The weight.
        /// </summary>
        private decimal? _weight;

        /// <summary>
        /// The length.
        /// </summary>
        private decimal? _length;

        /// <summary>
        /// The width.
        /// </summary>
        private decimal? _width;

        /// <summary>
        /// The height.
        /// </summary>
        private decimal? _height;

        /// <summary>
        /// The barcode.
        /// </summary>
        private string _barcode;

        /// <summary>
        /// The available.
        /// </summary>
        private bool _available;

        /// <summary>
        /// The track inventory.
        /// </summary>
        private bool _trackInventory;

        /// <summary>
        /// The _out of stock purchase.
        /// </summary>
        private bool _outOfStockPurchase;

        /// <summary>
        /// The taxable.
        /// </summary>
        private bool _taxable;

        /// <summary>
        /// The shippable.
        /// </summary>
        private bool _shippable;

        /// <summary>
        /// The download.
        /// </summary>
        private bool _download;

        /// <summary>
        /// The download media id.
        /// </summary>
        private int? _downloadMediaId;

        /// <summary>
        /// The version key.
        /// </summary>
        private Guid _versionKey;

        /// <summary>
        /// The catalog inventory collection.
        /// </summary>
        private CatalogInventoryCollection _catalogInventoryCollection;

        /// <summary>
        /// The detached content collection.
        /// </summary>
        private DetachedContentCollection<IProductVariantDetachedContent> _detachedContents; 

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBase"/> class.
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
        /// <param name="catalogInventoryCollection">
        /// The catalog inventory collection.
        /// </param>
        /// <param name="detachedContents">
        /// The detached Contents.
        /// </param>
        internal ProductBase(string name, string sku, decimal price, CatalogInventoryCollection catalogInventoryCollection, DetachedContentCollection<IProductVariantDetachedContent> detachedContents)
        {
            Ensure.ParameterNotNullOrEmpty(name, "name");
            Ensure.ParameterNotNullOrEmpty(sku, "sku");
            Ensure.ParameterNotNull(catalogInventoryCollection, "warehouseInventory");
            Ensure.ParameterNotNull(detachedContents, "detachedContents");
            _name = name;
            _sku = sku;
            _price = price;
            _catalogInventoryCollection = catalogInventoryCollection;
            _detachedContents = detachedContents;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBase"/> class.
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
        protected ProductBase(string name, string sku, decimal price)
            : this(name, sku, price, new CatalogInventoryCollection(), new DetachedContentCollection<IProductVariantDetachedContent>())
        {
        }
        

        /// <inheritdoc/>
        [DataMember]
        public IEnumerable<ICatalogInventory> CatalogInventories
        {
            get
            {
                return _catalogInventoryCollection;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public virtual DetachedContentCollection<IProductVariantDetachedContent> DetachedContents
        {
            get
            {
                return _detachedContents;
            }

            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _detachedContents = value;
                _detachedContents.CollectionChanged += DetachedContentsOnCollectionChanged;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Sku
        {
            get
            {
                return _sku;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _sku, _ps.Value.SkuSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal Price
        {
            get
            {
                return _price;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _price, _ps.Value.PriceSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? CostOfGoods
        {
            get
            {
                return _costOfGoods;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _costOfGoods, _ps.Value.CostOfGoodsSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? SalePrice
        {
            get
            {
                return _salePrice;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _salePrice, _ps.Value.SalePriceSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool OnSale
        {
            get
            {
                return _onSale;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _onSale, _ps.Value.OnSaleSelector);
            }
        }


        /// <inheritdoc/>
        [DataMember]
        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _manufacturer, _ps.Value.ManufacturerSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ManufacturerModelNumber
        {
            get
            {
                return _manufacturerModelNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _manufacturerModelNumber, _ps.Value.ManufacturerModelNumberSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _weight, _ps.Value.WeightSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Length
        {
            get
            {
                return _length;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _length, _ps.Value.LengthSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Width
        {
            get
            {
                return _width;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _width, _ps.Value.WidthSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal? Height
        {
            get
            {
                return _height;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _height, _ps.Value.HeightSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Barcode
        {
            get
            {
                return _barcode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _barcode, _ps.Value.BarcodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Available
        {
            get
            {
                return _available;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _available, _ps.Value.AvailableSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool TrackInventory
        {
            get
            {
                return _trackInventory;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _trackInventory, _ps.Value.TrackInventorySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get
            {
                return _outOfStockPurchase;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _outOfStockPurchase, _ps.Value.OutOfStockPurchaseSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Taxable
        {
            get
            {
                return _taxable;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _taxable, _ps.Value.TaxableSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Shippable
        {
            get
            {
                return _shippable;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shippable, _ps.Value.ShippableSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Download
        {
            get
            {
                return _download;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _download, _ps.Value.DownloadSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int? DownloadMediaId
        {
            get
            {
                return _downloadMediaId;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _downloadMediaId, _ps.Value.DownloadMediaIdSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid VersionKey
        {
            get
            {
                return _versionKey;                 
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _versionKey, _ps.Value.VersionKeySelector);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        internal CatalogInventoryCollection CatalogInventoryCollection
        {
            get
            {
                return _catalogInventoryCollection;
            }

            set
            {
                _catalogInventoryCollection = value;
                _catalogInventoryCollection.CollectionChanged += CatalogInventoryCollectionChanged;
            }
        }

        /// <summary>
        /// Handles the catalog inventory collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CatalogInventoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.WarehouseInventoryChangedSelector);
        }

        /// <summary>
        /// Handles the detached contents on collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="notifyCollectionChangedEventArgs">
        /// The notify collection changed event args.
        /// </param>
        private void DetachedContentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.OnPropertyChanged(_ps.Value.DetachedContentsSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The SKU selector.
            /// </summary>
            public readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Sku);

            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Name);

            /// <summary>
            /// The price selector.
            /// </summary>
            public readonly PropertyInfo PriceSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal>(x => x.Price);

            /// <summary>
            /// The cost of goods selector.
            /// </summary>
            public readonly PropertyInfo CostOfGoodsSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.CostOfGoods);

            /// <summary>
            /// The sale price selector.
            /// </summary>
            public readonly PropertyInfo SalePriceSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.SalePrice);

            /// <summary>
            /// The on sale selector.
            /// </summary>
            public readonly PropertyInfo OnSaleSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.OnSale);

            /// <summary>
            /// The manufacturer selector.
            /// </summary>
            public readonly PropertyInfo ManufacturerSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Manufacturer);

            /// <summary>
            /// The manufacturer model number selector.
            /// </summary>
            public readonly PropertyInfo ManufacturerModelNumberSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.ManufacturerModelNumber);

            /// <summary>
            /// The weight selector.
            /// </summary>
            public readonly PropertyInfo WeightSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Weight);

            /// <summary>
            /// The length selector.
            /// </summary>
            public readonly PropertyInfo LengthSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Length);

            /// <summary>
            /// The width selector.
            /// </summary>
            public readonly PropertyInfo WidthSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Width);

            /// <summary>
            /// The height selector.
            /// </summary>
            public readonly PropertyInfo HeightSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Height);

            /// <summary>
            /// The barcode selector.
            /// </summary>
            public readonly PropertyInfo BarcodeSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Barcode);

            /// <summary>
            /// The available selector.
            /// </summary>
            public readonly PropertyInfo AvailableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Available);

            /// <summary>
            /// The track inventory selector.
            /// </summary>
            public readonly PropertyInfo TrackInventorySelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.TrackInventory);

            /// <summary>
            /// The out of stock purchase selector.
            /// </summary>
            public readonly PropertyInfo OutOfStockPurchaseSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.OutOfStockPurchase);

            /// <summary>
            /// The taxable selector.
            /// </summary>
            public readonly PropertyInfo TaxableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Taxable);

            /// <summary>
            /// The shippable selector.
            /// </summary>
            public readonly PropertyInfo ShippableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Shippable);

            /// <summary>
            /// The download selector.
            /// </summary>
            public readonly PropertyInfo DownloadSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Download);

            /// <summary>
            /// The download media id selector.
            /// </summary>
            public readonly PropertyInfo DownloadMediaIdSelector = ExpressionHelper.GetPropertyInfo<ProductBase, int?>(x => x.DownloadMediaId);

            /// <summary>
            /// The version key selector.
            /// </summary>
            public readonly PropertyInfo VersionKeySelector = ExpressionHelper.GetPropertyInfo<ProductBase, Guid>(x => x.VersionKey);

            /// <summary>
            /// The warehouse inventory changed selector.
            /// </summary>
            public readonly PropertyInfo WarehouseInventoryChangedSelector = ExpressionHelper.GetPropertyInfo<ProductBase, CatalogInventoryCollection>(x => x.CatalogInventoryCollection);

            /// <summary>
            /// The detached contents selector.
            /// </summary>
            public readonly PropertyInfo DetachedContentsSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, DetachedContentCollection<IProductVariantDetachedContent>>(x => x.DetachedContents);
        }
    }
}