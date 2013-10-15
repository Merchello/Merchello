using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        internal ProductVariant(string name, string sku, decimal price)
            : this(Guid.Empty, new ProductAttributeCollection(), new WarehouseInventoryCollection(), false, name, sku, price)
        { }

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, string name, string sku, decimal price)
            : this(productKey, attributes, new WarehouseInventoryCollection(), false, name, sku, price)
        {}

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, WarehouseInventoryCollection warehouseInventory, string name, string sku, decimal price)
            : this(productKey, attributes, warehouseInventory, false, name, sku, price)
        { }

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, WarehouseInventoryCollection warehouseInventory, bool master, string name, string sku, decimal price)
            : base(name, sku, price, warehouseInventory)
        {
            Mandate.ParameterNotNull(attributes, "attributes");
            Mandate.ParameterNotNull(warehouseInventory, "warehouseInventory");
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
        [DataMember]
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

        /// <summary>
        /// Associates a product variant with a warehouse
        /// </summary>
        /// <param name="warehouseId">The 'unique' id of the <see cref="IWarehouse"/></param>
        public void AddToWarehouse(int warehouseId)
        {
            WarehouseInventory.Add(new WarehouseInventory(warehouseId, Key));
        }

    }
}