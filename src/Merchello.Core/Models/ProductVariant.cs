using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product variant
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ProductVariant : ProductBase, IProductVariant
    {
        private Guid _productKey;
        private ProductAttributeCollection _attibutes;
        private bool _master;
        private int _examineId = 1;

        internal ProductVariant(string name, string sku, decimal price)
            : this(Guid.Empty, new ProductAttributeCollection(), new CatalogInventoryCollection(), false, name, sku, price)
        { }

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, string name, string sku, decimal price)
            : this(productKey, attributes, new CatalogInventoryCollection(), false, name, sku, price)
        {}

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, CatalogInventoryCollection catalogInventoryCollection, string name, string sku, decimal price)
            : this(productKey, attributes, catalogInventoryCollection, false, name, sku, price)
        { }

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, CatalogInventoryCollection catalogInventoryCollection, bool master, string name, string sku, decimal price)
            : base(name, sku, price, catalogInventoryCollection)
        {
            Mandate.ParameterNotNull(attributes, "attributes");
            Mandate.ParameterNotNull(catalogInventoryCollection, "warehouseInventory");
            _productKey = productKey;
            _attibutes = attributes;
            _master = master;
        }

        private static readonly PropertyInfo ProductKeySelector = ExpressionHelper.GetPropertyInfo<ProductVariant, Guid>(x => x.ProductKey);        
        private static readonly PropertyInfo MasterSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, bool>(x => x.Master);
        private static readonly PropertyInfo AttributesChangedSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, ProductAttributeCollection>(x => x.ProductAttributes);

        private void ProductAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(AttributesChangedSelector);
        }

        /// <summary>
        /// The key for the defining product
        /// </summary>
        [DataMember]
        public Guid ProductKey
        {
            get
            {
                return _productKey;
            }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _productKey = value;
                    return _productKey;
                }, _productKey, ProductKeySelector);
            }
        }

        /// <summary>
        /// The collection of attributes that makes this variant different from other variants of the same product
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<IProductAttribute> Attributes 
        {
            get { return _attibutes; }
            
        }

        [IgnoreDataMember]
        internal ProductAttributeCollection ProductAttributes
        {
            get { return _attibutes; }
            set
            {
                _attibutes = value;
                _attibutes.CollectionChanged += ProductAttributesChanged;
            }
        }

        /// <summary>
        /// True/false indicating whether or not this variant is the "master" variant for the product.  All products (even products without options) have a master variant.
        /// </summary>
        [IgnoreDataMember]
        internal bool Master
        {
            get { return _master; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _master = value;
                    return _master;
                }, _master, MasterSelector);
            }
        }

        [IgnoreDataMember]
        internal int ExamineId {
            get { return  _examineId; }
            set { _examineId = value;  }
        }

        /// <summary>
        /// Returns the total (sum) of inventory "counts" accross all associated warehouses
        /// </summary>
        /// <returns></returns>
        [IgnoreDataMember]
        public int TotalInventoryCount
        {
            get { return CatalogInventories.Sum(x => x.Count); }
        }
    }
}