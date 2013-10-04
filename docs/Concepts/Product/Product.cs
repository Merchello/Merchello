using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Product : KeyEntity, IProduct
    {
        private bool _singular = true;
        private bool _outOfStockPurchase;
        private readonly IProductItem _singleItem;
        private readonly ProductItemCollection _productItems;
        private readonly OptionCollection _options;

        public Product()
            : this(new ProductItemCollection(), new OptionCollection())
        {}

        public Product(ProductItemCollection productItems, OptionCollection options)
        {
            Mandate.ParameterNotNull(productItems, "productItems");
            Mandate.ParameterNotNull(options, "options");
            Mandate.ParameterCondition(GetSingleItem() != null, "productItems must contain a template item");

            _productItems = productItems;
            _options = options;
            _singleItem = GetSingleItem();
        }

        private static readonly PropertyInfo SingularSelector = ExpressionHelper.GetPropertyInfo<Product, bool>(x => x.Singular);
        private static readonly PropertyInfo OutOfStockPurchaseSelector = ExpressionHelper.GetPropertyInfo<Product, bool>(x => x.OutOfStockPurchase);

        [DataMember]
        public bool Singular
        {
            get { return _singular; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _singular = value;
                    return _singular;
                }, _singular, SingularSelector); 
            }
        }

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

        [DataMember]
        public ProductItemCollection ProductItems
        {
            get { return _productItems; }
        }

        [DataMember]
        public OptionCollection Options
        {
            get { return _options; }            
        }

        internal IProductItem GetSingleItem()
        {
            return ProductItems.First(x => x.SingleItem);
        }
       

        #region Singular / Tempalte Item

        [IgnoreDataMember]
        public Guid ProductKey {
            get { return Key; }
        }

        [DataMember]
        public string Sku
        {
            get { return _singleItem.Sku; } 
            set { _singleItem.Sku = value; }
        }

        [DataMember]
        public decimal Price
        {
            get { return _singleItem.Price; }
            set { _singleItem.Price = value; }
        }

        [DataMember]
        public decimal CostOfGoods 
        {
            get { return _singleItem.CostOfGoods; }
            set { _singleItem.CostOfGoods = value; }
        }

        [DataMember]
        public decimal SalePrice
        {
            get { return _singleItem.SalePrice; }
            set { _singleItem.SalePrice = value; }
        }

        [DataMember]
        public bool OnSale
        {
            get { return _singleItem.OnSale; }
            set { _singleItem.OnSale = value; }
        }

        [DataMember]
        public decimal Weight
        {
            get { return _singleItem.Weight; }
            set { _singleItem.Weight = value; }
        }

        [DataMember]
        public decimal Length
        {
            get { return _singleItem.Length; }
            set { _singleItem.Length = value; }
        }

        [DataMember]
        public decimal Width
        {
            get { return _singleItem.Width; }
            set { _singleItem.Width = value; }
        }

        [DataMember]
        public decimal Height
        {
            get { return _singleItem.Height; }
            set { _singleItem.Height = value; }
        }

        [DataMember]
        public string Barcode
        {
            get { return _singleItem.Barcode; }
            set { _singleItem.Barcode = value; }
        }

        [DataMember]
        public bool Available
        {
            get { return _singleItem.Available; }
            set { _singleItem.Available = value; }
        }

        [DataMember]
        public bool TrackInventory
        {
            get { return _singleItem.TrackInventory; }
            set { _singleItem.TrackInventory = value; }
        }

        [DataMember]
        public bool Taxable
        {
            get { return _singleItem.Taxable; }
            set { _singleItem.Taxable = value; }
        }

        [DataMember]
        public bool Shippable
        {
            get { return _singleItem.Shippable; }
            set { _singleItem.Shippable = value; }
        }

        [DataMember]
        public bool Download
        {
            get { return _singleItem.Download; }
            set { _singleItem.Download = value; }
        }

        [DataMember]
        public bool DownloadMediaId
        {
            get { return _singleItem.DownloadMediaId; }
            set { _singleItem.DownloadMediaId = value; }
        }

        [DataMember]
        public bool SingleItem {
            get { return _singular; }
        }

        [IgnoreDataMember]
        public ProductAttributeCollection Attributes
        {
            get { return _singleItem.Attributes; }
        }

        #endregion
    }
}