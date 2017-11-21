namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.DetachedContent;

    /// <summary>
    /// Defines a product variant
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ProductVariant : ProductBase, IProductVariant
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The product key.
        /// </summary>
        private Guid _productKey;

        /// <summary>
        /// The attributes.
        /// </summary>
        private ProductAttributeCollection _attibutes;


        /// <summary>
        /// The value indicating whether or not this is the master variant.
        /// </summary>
        private bool _master;

        /// <summary>
        /// The examine id.
        /// </summary>
        private int _examineId = 1;

        #region Contstructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariant"/> class.
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
        internal ProductVariant(string name, string sku, decimal price)
            : this(Guid.Empty, new ProductAttributeCollection(), new CatalogInventoryCollection(), false, name, sku, price)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariant"/> class.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        internal ProductVariant(
            Guid productKey,
            ProductAttributeCollection attributes,
            string name,
            string sku,
            decimal price)
            : this(productKey, attributes, new CatalogInventoryCollection(), false, name, sku, price)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariant"/> class.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="catalogInventoryCollection">
        /// The catalog inventory collection.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        internal ProductVariant(
            Guid productKey,
            ProductAttributeCollection attributes,
            CatalogInventoryCollection catalogInventoryCollection,
            string name,
            string sku,
            decimal price)
            : this(productKey, attributes, catalogInventoryCollection, false, name, sku, price)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariant"/> class.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="catalogInventoryCollection">
        /// The catalog inventory collection.
        /// </param>
        /// <param name="master">
        /// The master.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, CatalogInventoryCollection catalogInventoryCollection, bool master, string name, string sku, decimal price)
            : this(productKey, attributes, catalogInventoryCollection, new DetachedContentCollection<IProductVariantDetachedContent>(), false, name, sku, price)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariant"/> class.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="catalogInventoryCollection">
        /// The catalog inventory collection.
        /// </param>
        /// <param name="detachedContents">
        /// The detached contents.
        /// </param>
        /// <param name="master">
        /// The master.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        internal ProductVariant(Guid productKey, ProductAttributeCollection attributes, CatalogInventoryCollection catalogInventoryCollection, DetachedContentCollection<IProductVariantDetachedContent> detachedContents,  bool master, string name, string sku, decimal price)
            : base(name, sku, price, catalogInventoryCollection, detachedContents)
        {
            Ensure.ParameterNotNull(attributes, "attributes");
            Ensure.ParameterNotNull(catalogInventoryCollection, "warehouseInventory");
           
            _productKey = productKey;
            _attibutes = attributes;
            _master = master;
        }

        #endregion

        /// <inheritdoc/>
        [DataMember]
        public Guid ProductKey
        {
            get
            {
                return _productKey;
            }
            set
            {
                SetPropertyValueAndDetectChanges(value, ref _productKey, _ps.Value.ProductKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int TotalInventoryCount
        {
            get
            {
                return TrackInventory ? CatalogInventories.Sum(x => x.Count) : 0;
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public int ExamineId
        {
            get { return _examineId; }
            internal set { _examineId = value; }
        }


        /// <inheritdoc/>
        [IgnoreDataMember]
        public IEnumerable<IProductAttribute> Attributes 
        {
            get { return _attibutes; }            
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public bool Master
        {
            get
            {
                return _master;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _master, _ps.Value.MasterSelector);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        internal ProductAttributeCollection ProductAttributes
        {
            get
            {
                return _attibutes;
            }

            set
            {
                _attibutes = value;
                _attibutes.CollectionChanged += ProductAttributesChanged;
            }
        }

        /// <summary>
        /// Handles the product attributes collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProductAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.AttributesChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The product key selector.
            /// </summary>
            public readonly PropertyInfo ProductKeySelector = ExpressionHelper.GetPropertyInfo<ProductVariant, Guid>(x => x.ProductKey);

            /// <summary>
            /// The master selector.
            /// </summary>
            public readonly PropertyInfo MasterSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, bool>(x => x.Master);

            /// <summary>
            /// The attributes changed selector.
            /// </summary>
            public readonly PropertyInfo AttributesChangedSelector = ExpressionHelper.GetPropertyInfo<ProductVariant, ProductAttributeCollection>(x => x.ProductAttributes);
        }
    }
}