namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;

    /// <summary>
    /// Represents an abstract class for base Product properties and methods
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class ProductBase : Entity, IProductBase
    {
        #region Fields

        /// <summary>
        /// The sku selector.
        /// </summary>
        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Sku);

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Name);

        /// <summary>
        /// The price selector.
        /// </summary>
        private static readonly PropertyInfo PriceSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal>(x => x.Price);

        /// <summary>
        /// The cost of goods selector.
        /// </summary>
        private static readonly PropertyInfo CostOfGoodsSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.CostOfGoods);

        /// <summary>
        /// The sale price selector.
        /// </summary>
        private static readonly PropertyInfo SalePriceSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.SalePrice);

        /// <summary>
        /// The on sale selector.
        /// </summary>
        private static readonly PropertyInfo OnSaleSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.OnSale);

        /// <summary>
        /// The manufacturer selector.
        /// </summary>
        private static readonly PropertyInfo ManufacturerSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Manufacturer);

        /// <summary>
        /// The manufacturer model number selector.
        /// </summary>
        private static readonly PropertyInfo ManufacturerModelNumberSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.ManufacturerModelNumber);

        /// <summary>
        /// The weight selector.
        /// </summary>
        private static readonly PropertyInfo WeightSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Weight);

        /// <summary>
        /// The length selector.
        /// </summary>
        private static readonly PropertyInfo LengthSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Length);

        /// <summary>
        /// The width selector.
        /// </summary>
        private static readonly PropertyInfo WidthSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Width);

        /// <summary>
        /// The height selector.
        /// </summary>
        private static readonly PropertyInfo HeightSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Height);

        /// <summary>
        /// The barcode selector.
        /// </summary>
        private static readonly PropertyInfo BarcodeSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Barcode);

        /// <summary>
        /// The available selector.
        /// </summary>
        private static readonly PropertyInfo AvailableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Available);

        /// <summary>
        /// The track inventory selector.
        /// </summary>
        private static readonly PropertyInfo TrackInventorySelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.TrackInventory);

        /// <summary>
        /// The out of stock purchase selector.
        /// </summary>
        private static readonly PropertyInfo OutOfStockPurchaseSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.OutOfStockPurchase);

        /// <summary>
        /// The taxable selector.
        /// </summary>
        private static readonly PropertyInfo TaxableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Taxable);

        /// <summary>
        /// The shippable selector.
        /// </summary>
        private static readonly PropertyInfo ShippableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Shippable);

        /// <summary>
        /// The download selector.
        /// </summary>
        private static readonly PropertyInfo DownloadSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Download);

        /// <summary>
        /// The download media id selector.
        /// </summary>
        private static readonly PropertyInfo DownloadMediaIdSelector = ExpressionHelper.GetPropertyInfo<ProductBase, int?>(x => x.DownloadMediaId);

        /// <summary>
        /// The version key selector.
        /// </summary>
        private static readonly PropertyInfo VersionKeySelector = ExpressionHelper.GetPropertyInfo<ProductBase, Guid>(x => x.VersionKey);

        /// <summary>
        /// The warehouse inventory changed selector.
        /// </summary>
        private static readonly PropertyInfo WarehouseInventoryChangedSelector = ExpressionHelper.GetPropertyInfo<ProductBase, CatalogInventoryCollection>(x => x.CatalogInventoryCollection);


        /// <summary>
        /// The sku.
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

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="catalogInventoryCollection">
        /// The catalog inventory collection.
        /// </param>
        internal ProductBase(string name, string sku, decimal price, CatalogInventoryCollection catalogInventoryCollection)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
            Mandate.ParameterNotNull(catalogInventoryCollection, "warehouseInventory");
            _name = name;
            _sku = sku;
            _price = price;
            _catalogInventoryCollection = catalogInventoryCollection;            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        protected ProductBase(string name, string sku, decimal price)
            : this(name, sku, price, new CatalogInventoryCollection())
        {
        }
        

        /// <summary>
        /// Gets a Product variant inventory accross all warehouses
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<ICatalogInventory> CatalogInventories
        {
            get { return _catalogInventoryCollection; }
        }       

        /// <summary>
        /// Gets or sets sku associated with the Product
        /// </summary>
        [DataMember]
        public string Sku
        {
            get
            {
                return _sku;
            }

            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _sku = value;
                    return _sku;
                }, _sku, SkuSelector);
            }
        }

        /// <summary>
        /// The name associated with the Product
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// The price associated with the Product
        /// </summary>
        [DataMember]
        public decimal Price
        {
            get { return _price; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _price = value;
                    return _price;
                }, _price, PriceSelector);
            }
        }

        /// <summary>
        /// The costOfGoods associated with the Product
        /// </summary>
        [DataMember]
        public decimal? CostOfGoods
        {
            get { return _costOfGoods; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _costOfGoods = value;
                    return _costOfGoods;
                }, _costOfGoods, CostOfGoodsSelector);
            }
        }

        /// <summary>
        /// The salePrice associated with the Product
        /// </summary>
        [DataMember]
        public decimal? SalePrice
        {
            get { return _salePrice; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _salePrice = value;
                    return _salePrice;
                }, _salePrice, SalePriceSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not this product is onsale
        /// </summary>
        [DataMember]
        public bool OnSale
        {
            get { return _onSale; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _onSale = value;
                    return _onSale;
                }, _onSale, OnSaleSelector);
            }
        }


        /// <summary>
        /// The manufacturer of the product
        /// </summary>
        [DataMember]
        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _manufacturer = value;
                    return _manufacturer;
                }, _manufacturer, ManufacturerSelector);
            }
        }

        /// <summary>
        /// The manufacturer model number of the product
        /// </summary>
        [DataMember]
        public string ManufacturerModelNumber
        {
            get { return _manufacturerModelNumber; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _manufacturerModelNumber = value;
                    return _manufacturerModelNumber;
                }, _manufacturerModelNumber, ManufacturerModelNumberSelector);
            }
        }


        /// <summary>
        /// The weight associated with the Product
        /// </summary>
        [DataMember]
        public decimal? Weight
        {
            get { return _weight; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _weight = value;
                    return _weight;
                }, _weight, WeightSelector);
            }
        }

        /// <summary>
        /// The length associated with the Product
        /// </summary>
        [DataMember]
        public decimal? Length
        {
            get { return _length; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _length = value;
                    return _length;
                }, _length, LengthSelector);
            }
        }

        /// <summary>
        /// The width associated with the Product
        /// </summary>
        [DataMember]
        public decimal? Width
        {
            get { return _width; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _width = value;
                    return _width;
                }, _width, WidthSelector);
            }
        }

        /// <summary>
        /// The height associated with the Product
        /// </summary>
        [DataMember]
        public decimal? Height
        {
            get { return _height; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _height = value;
                    return _height;
                }, _height, HeightSelector);
            }
        }

        /// <summary>
        /// The barcode of the product
        /// </summary>
        [DataMember]
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _barcode = value;
                    return _barcode;
                }, _barcode, BarcodeSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not this product is available
        /// </summary>
        [DataMember]
        public bool Available
        {
            get { return _available; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _available = value;
                    return _available;
                }, _available, AvailableSelector);
            }
        }

        /// <summary>
        /// True/false indicating whether or not to track inventory on this product
        /// </summary>
        [DataMember]
        public bool TrackInventory
        {
            get { return _trackInventory; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _trackInventory = value;
                    return _trackInventory;
                }, _trackInventory, TrackInventorySelector);
            }
        }

        /// <summary>
        /// True/false indicating wether or not this product can be purchased when inventory levels are 
        /// 0 or below.
        /// </summary>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get { return _outOfStockPurchase; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _outOfStockPurchase = value;
                    return _outOfStockPurchase;
                }, _outOfStockPurchase, OutOfStockPurchaseSelector);
            }
        }

        /// <summary>
        /// The taxable associated with the Product
        /// </summary>
        [DataMember]
        public bool Taxable
        {
            get { return _taxable; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _taxable = value;
                    return _taxable;
                }, _taxable, TaxableSelector);
            }
        }

        /// <summary>
        /// The shippable associated with the Product
        /// </summary>
        [DataMember]
        public bool Shippable
        {
            get { return _shippable; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _shippable = value;
                    return _shippable;
                }, _shippable, ShippableSelector);
            }
        }

        /// <summary>
        /// The download associated with the Product
        /// </summary>
        [DataMember]
        public bool Download
        {
            get { return _download; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _download = value;
                    return _download;
                }, _download, DownloadSelector);
            }
        }

        /// <summary>
        /// The downloadUrl associated with the Product
        /// </summary>
        [DataMember]
        public int? DownloadMediaId
        {
            get { return _downloadMediaId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _downloadMediaId = value;
                    return _downloadMediaId;
                }, _downloadMediaId, DownloadMediaIdSelector);
            }
        }

        /// <summary>
        /// Gets the version key.
        /// </summary>
        [DataMember]
        public Guid VersionKey
        {
            get
            {
                return _versionKey;                 
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                o =>
                {
                    _versionKey = value;
                    return _versionKey;
                }, 
                _versionKey, 
                VersionKeySelector);
            }
        }

        [IgnoreDataMember]
        internal CatalogInventoryCollection CatalogInventoryCollection
        {
            get { return _catalogInventoryCollection; }
            set
            {
                _catalogInventoryCollection = value;
                _catalogInventoryCollection.CollectionChanged += CatalogInventoryCollectionChanged;
            }
        }

        private void CatalogInventoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(WarehouseInventoryChangedSelector);
        }
 
    }
}