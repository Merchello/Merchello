using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping.Models
{
    /// <summary>
    /// Represents an abstract class for base Product properties and methods
    /// </summary>
    public abstract class ProductBase : KeyEntity, IProductBase
    {
        private string _sku;
        private string _name;
        private decimal _price;
        private decimal? _costOfGoods;
        private bool _onSale;
        private decimal? _salePrice;
        private decimal? _weight;
        private decimal? _length;
        private decimal? _width;
        private decimal? _height;
        private string _barcode;
        private bool _available;
        private bool _trackInventory;
        private bool _outOfStockPurchase;
        private bool _taxable;
        private bool _shippable;
        private bool _download;
        private int? _downloadMediaId;

        private readonly InventoryCollection _inventory;

        protected ProductBase(string name, string sku, decimal price)
            : this(name, sku, price, new InventoryCollection())
        { }
        
        internal ProductBase(string name, string sku, decimal price, InventoryCollection inventory)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(sku, "sku");
            Mandate.ParameterNotNull(inventory, "inventory");
            _name = name;
            _sku = sku;
            _price = price;
            _inventory = inventory;
            
        }

        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Sku);
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Name);
        private static readonly PropertyInfo PriceSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal>(x => x.Price);
        private static readonly PropertyInfo CostOfGoodsSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.CostOfGoods);
        private static readonly PropertyInfo SalePriceSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.SalePrice);
        private static readonly PropertyInfo OnSaleSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.OnSale);
        private static readonly PropertyInfo WeightSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Weight);
        private static readonly PropertyInfo LengthSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Length);
        private static readonly PropertyInfo WidthSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Width);
        private static readonly PropertyInfo HeightSelector = ExpressionHelper.GetPropertyInfo<ProductBase, decimal?>(x => x.Height);
        private static readonly PropertyInfo BarcodeSelector = ExpressionHelper.GetPropertyInfo<ProductBase, string>(x => x.Barcode);
        private static readonly PropertyInfo AvailableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Available);
        private static readonly PropertyInfo TrackInventorySelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.TrackInventory);
        private static readonly PropertyInfo OutOfStockPurchaseSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.OutOfStockPurchase);
        private static readonly PropertyInfo TaxableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Taxable);
        private static readonly PropertyInfo ShippableSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Shippable);
        private static readonly PropertyInfo DownloadSelector = ExpressionHelper.GetPropertyInfo<ProductBase, bool>(x => x.Download);
        private static readonly PropertyInfo DownloadMediaIdSelector = ExpressionHelper.GetPropertyInfo<ProductBase, int?>(x => x.DownloadMediaId);
        




        /// <summary>
        /// The sku associated with the Product
        /// </summary>
        [DataMember]
        public string Sku
        {
            get { return _sku; }
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
        /// Product variant inventory accross all warehouses
        /// </summary>
        [DataMember]
        public InventoryCollection Inventory 
        {
            get { return _inventory; }
        }
    }
}