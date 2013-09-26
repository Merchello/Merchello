using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Product : KeyEntity, IProduct
    {
        private string _sku;
        private string _name;
        private decimal _price;
        private decimal? _costOfGoods;
        private decimal? _salePrice;
        private decimal? _weight;
        private decimal? _length;
        private decimal? _width;
        private decimal? _height;
        private bool _taxable;
        private bool _shippable;
        private bool _download;
        private string _downloadUrl;
        private bool _template;

        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.Sku);  
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.Name);  
        private static readonly PropertyInfo PriceSelector = ExpressionHelper.GetPropertyInfo<Product, decimal>(x => x.Price);  
        private static readonly PropertyInfo CostOfGoodsSelector = ExpressionHelper.GetPropertyInfo<Product, decimal?>(x => x.CostOfGoods);  
        private static readonly PropertyInfo SalePriceSelector = ExpressionHelper.GetPropertyInfo<Product, decimal?>(x => x.SalePrice);  
        private static readonly PropertyInfo WeightSelector = ExpressionHelper.GetPropertyInfo<Product, decimal?>(x => x.Weight);  
        private static readonly PropertyInfo LengthSelector = ExpressionHelper.GetPropertyInfo<Product, decimal?>(x => x.Length);  
        private static readonly PropertyInfo WidthSelector = ExpressionHelper.GetPropertyInfo<Product, decimal?>(x => x.Width);  
        private static readonly PropertyInfo HeightSelector = ExpressionHelper.GetPropertyInfo<Product, decimal?>(x => x.Height);          
        private static readonly PropertyInfo TaxableSelector = ExpressionHelper.GetPropertyInfo<Product, bool>(x => x.Taxable);  
        private static readonly PropertyInfo ShippableSelector = ExpressionHelper.GetPropertyInfo<Product, bool>(x => x.Shippable);  
        private static readonly PropertyInfo DownloadSelector = ExpressionHelper.GetPropertyInfo<Product, bool>(x => x.Download);  
        private static readonly PropertyInfo DownloadUrlSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.DownloadUrl);  
        private static readonly PropertyInfo TemplateSelector = ExpressionHelper.GetPropertyInfo<Product, bool>(x => x.Template);  
  
    
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
        public string DownloadUrl
        {
            get { return _downloadUrl; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _downloadUrl = value;
                    return _downloadUrl;
                }, _downloadUrl, DownloadUrlSelector); 
            }
        }
    
        /// <summary>
        /// The template associated with the Product
        /// </summary>
        [DataMember]
        public bool Template
        {
            get { return _template; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _template = value;
                    return _template;
                }, _template, TemplateSelector); 
            }
        }                    
    }
}