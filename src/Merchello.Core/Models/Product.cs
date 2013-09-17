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
        private decimal _costOfGoods;
        private decimal _salePrice;
        private string _brief;
        private string _description;
        private bool _taxable;
        private bool _shippable;
        private bool _download;
        private string _downloadUrl;
        private bool _template;

        private static readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.Sku);  
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.Name);  
        private static readonly PropertyInfo PriceSelector = ExpressionHelper.GetPropertyInfo<Product, decimal>(x => x.Price);  
        private static readonly PropertyInfo CostOfGoodsSelector = ExpressionHelper.GetPropertyInfo<Product, decimal>(x => x.CostOfGoods);  
        private static readonly PropertyInfo SalePriceSelector = ExpressionHelper.GetPropertyInfo<Product, decimal>(x => x.SalePrice);  
        private static readonly PropertyInfo BriefSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.Brief);  
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<Product, string>(x => x.Description);  
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
    public decimal CostOfGoods
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
    public decimal SalePrice
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
    /// The brief associated with the Product
    /// </summary>
    [DataMember]
    public string Brief
    {
        get { return _brief; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _brief = value;
                    return _brief;
                }, _brief, BriefSelector); 
            }
    }
    
    /// <summary>
    /// The description associated with the Product
    /// </summary>
    [DataMember]
    public string Description
    {
        get { return _description; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _description = value;
                    return _description;
                }, _description, DescriptionSelector); 
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