using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core;

namespace Merchello.Tests.Base.Prototyping.Models
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
        private bool _template;

        public ProductVariant(string name, string sku, decimal price)
            : this(Guid.Empty, new ProductAttributeCollection(), new InventoryCollection(), false, name, sku, price)
        { }

        public ProductVariant(Guid productKey, ProductAttributeCollection attributes, string name, string sku, decimal price)
            : this(productKey, attributes, new InventoryCollection(), false, name, sku, price)
        {}

        public ProductVariant(Guid productKey, ProductAttributeCollection attributes, InventoryCollection inventory, string name, string sku, decimal price)
            : this(productKey, attributes, inventory, false, name, sku, price)
        { }

        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, InventoryCollection inventory, bool template, string name, string sku, decimal price)
            : base(name, sku, price)
        {
            Mandate.ParameterNotNull(attributes, "attributes");
            Mandate.ParameterNotNull(inventory, "inventory");
            _productKey = productKey;
            _attibutes = attributes;
            _template = template;
        }

        private static readonly PropertyInfo ProductKeySelector = ExpressionHelper.GetPropertyInfo<ProductVariant, Guid>(x => x.ProductKey);
        private static readonly PropertyInfo AttributesSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, ProductAttributeCollection>(x => x.Attributes);
        private static readonly PropertyInfo TemplateSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, bool>(x => x.Template);

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
            internal set
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
        public ProductAttributeCollection Attributes 
        {
            get { return _attibutes; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _attibutes = value;
                    return _attibutes;
                }, _attibutes, AttributesSelector);
            }
        }

        /// <summary>
        /// The template associated with the Product
        /// </summary>
        [IgnoreDataMember]
        internal bool Template
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