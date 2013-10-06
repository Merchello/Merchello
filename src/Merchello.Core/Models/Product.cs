using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Tests.Base.Prototyping.Models;

namespace Merchello.Core.Models
{
    internal class Product : KeyEntity, IProduct
    {
        
        private readonly IProductVariant _variantTemplate;
        private readonly ProductOptionCollection _productOptionCollection;

        public Product(IProductVariant variantTemplate)
            : this(variantTemplate, new ProductOptionCollection())
        {}

        public Product(IProductVariant variantTemplate, ProductOptionCollection productOptionCollection)
        {
            Mandate.ParameterNotNull(variantTemplate, "groupTemplate");
            Mandate.ParameterNotNull(productOptionCollection, "optionCollection");

            _variantTemplate = variantTemplate;
            _productOptionCollection = productOptionCollection;
        }


        /// <summary>
        /// True/false indicating whether or not this group defines options
        /// </summary>
        [DataMember]
        public bool DefinesOptions
        {
            get { return _productOptionCollection.Any(); }
            
        }

        /// <summary>
        /// The options that define the product attributes which 
        /// </summary>
        [DataMember]
        public ProductOptionCollection ProductOptions 
        {
            get { return _productOptionCollection; }
        }


        #region Variant Template

        internal IProductVariant ProductVariantTemplate
        {
            get { return _variantTemplate; }
        }

        /// <summary>
        /// Exposes the product variant template's key
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey
        {
            get { return _variantTemplate.Key; }
        }

        /// <summary>
        /// Exposes the product variant template's name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _variantTemplate.Name; }
            set { _variantTemplate.Name = value; }
        }

        /// <summary>
        /// Exposes the product variant template's sku
        /// </summary>
        [DataMember]
        public string Sku
        {
            get { return _variantTemplate.Sku; }
            set { _variantTemplate.Sku = value; }
        }

        /// <summary>
        /// Exposes the product variant template's price
        /// </summary>
        [DataMember]
        public decimal Price
        {
            get { return _variantTemplate.Price; }
            set { _variantTemplate.Price = value; }
        }

        /// <summary>
        /// Exposes the product variant template's cost of goods
        /// </summary>
        [DataMember]
        public decimal? CostOfGoods
        {
            get { return _variantTemplate.CostOfGoods; }
            set { _variantTemplate.CostOfGoods = value; }
        }

        /// <summary>
        /// Exposes the product variant template's sale price
        /// </summary>
        [DataMember]
        public decimal? SalePrice
        {
            get { return _variantTemplate.SalePrice; }
            set { _variantTemplate.SalePrice = value; }
        }

        /// <summary>
        /// Exposes the product variant template's on sale value
        /// </summary>
        [DataMember]
        public bool OnSale
        {
            get { return _variantTemplate.OnSale; }
            set { _variantTemplate.OnSale = value; }
        }

        /// <summary>
        /// Exposes the product variant template's weight
        /// </summary>
        [DataMember]
        public decimal? Weight
        {
            get { return _variantTemplate.Weight; }
            set { _variantTemplate.Weight = value; }
        }

        /// <summary>
        /// Exposes the product variant template's length
        /// </summary>
        [DataMember]
        public decimal? Length
        {
            get { return _variantTemplate.Length; }
            set { _variantTemplate.Length = value; }
        }

        /// <summary>
        /// Exposes the product variant template's width
        /// </summary>
        [DataMember]
        public decimal? Width
        {
            get { return _variantTemplate.Width; }
            set { _variantTemplate.Width = value; }
        }

        /// <summary>
        /// Exposes the product variant template's height
        /// </summary>
        [DataMember]
        public decimal? Height
        {
            get { return _variantTemplate.Height; }
            set { _variantTemplate.Height = value; }
        }

        /// <summary>
        /// Exposes the product variant template's barcode
        /// </summary>
        [DataMember]
        public string Barcode
        {
            get { return _variantTemplate.Barcode; }
            set { _variantTemplate.Barcode = value; }
        }

        /// <summary>
        /// Exposes the product variant template's available value
        /// </summary>
        [DataMember]
        public bool Available
        {
            get { return _variantTemplate.Available; }
            set { _variantTemplate.Available = value; }
        }

        /// <summary>
        /// Exposes the product variant template's trackinventory value
        /// </summary>
        [DataMember]
        public bool TrackInventory
        {
            get { return _variantTemplate.TrackInventory; }
            set { _variantTemplate.TrackInventory = value; }
        }

        /// <summary>
        /// Exposes the product variant template's outofstockpurchase value
        /// </summary>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get { return _variantTemplate.OutOfStockPurchase; }
            set { _variantTemplate.OutOfStockPurchase = value; }
        }

        /// <summary>
        /// Exposes the product variant template's taxable value
        /// </summary>
        [DataMember]
        public bool Taxable
        {
            get { return _variantTemplate.Taxable; }
            set { _variantTemplate.Taxable = value; }
        }

        /// <summary>
        /// Exposes the product variant template's shippable value
        /// </summary>
        [DataMember]
        public bool Shippable
        {
            get { return _variantTemplate.Shippable; }
            set { _variantTemplate.Shippable = value; }
        }

        /// <summary>
        /// Exposes the product variant template's download value
        /// </summary>
        [DataMember]
        public bool Download
        {
            get { return _variantTemplate.Download; }
            set { _variantTemplate.Download = value; }
        }

        /// <summary>
        /// Exposes the product variant template's downloadmediaid value
        /// </summary>
        [DataMember]
        public int? DownloadMediaId
        {
            get { return _variantTemplate.DownloadMediaId; }
            set { _variantTemplate.DownloadMediaId = value; }
        }

        /// <summary>
        /// Exposes the product variant template's inventory collection
        /// </summary>
        [DataMember]
        public InventoryCollection Inventory
        {
            get { return _variantTemplate.Inventory; }
        }

        #endregion

        public override void ResetDirtyProperties()
        {
            base.ResetDirtyProperties();
            _variantTemplate.ResetDirtyProperties();
        }

        internal override void AddingEntity()
        {
            base.AddingEntity();
            ((ProductVariant) _variantTemplate).Template = true;
            ((ProductVariant) _variantTemplate).AddingEntity();
        }

        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            ((ProductVariant)_variantTemplate).UpdatingEntity();
        }


    }
}