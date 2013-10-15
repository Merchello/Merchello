using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    internal class Product : KeyEntity, IProduct
    {
        
        private readonly IProductVariant _variantMaster;
        private ProductOptionCollection _productOptions;
        private ProductVariantCollection _productVariants;

        public Product(IProductVariant variantMaster)
            : this(variantMaster, new ProductOptionCollection(), new ProductVariantCollection())
        {}

        public Product(IProductVariant variantMaster, ProductOptionCollection productOptions, ProductVariantCollection productVariants)
        {
            Mandate.ParameterNotNull(variantMaster, "variantMaster");
            Mandate.ParameterNotNull(productOptions, "optionCollection");
            Mandate.ParameterNotNull(productVariants, "productVariants");

            _variantMaster = variantMaster;
            _productOptions = productOptions;
            _productVariants = productVariants;
        }

        private static readonly PropertyInfo ProductOptionsChangedSelector = ExpressionHelper.GetPropertyInfo<Product, ProductOptionCollection>(x => x.ProductOptions);
        private static readonly PropertyInfo ProductVariantsChangedSelector = ExpressionHelper.GetPropertyInfo<Product, ProductVariantCollection>(x => x.ProductVariants);

        private void ProductOptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ProductOptionsChangedSelector);
        }

        private void ProductVariantsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ProductVariantsChangedSelector);
        }

        #region Overrides IProduct
        

        /// <summary>
        /// True/false indicating whether or not this group defines options
        /// </summary>
        [DataMember]
        public bool DefinesOptions
        {
            get { return _productOptions.Any(); }
            
        }

        /// <summary>
        /// The options that define the product attributes which 
        /// </summary>
        [DataMember]
        public ProductOptionCollection ProductOptions 
        {
            get { return _productOptions; }
            set { 
                _productOptions = value;
                _productOptions.CollectionChanged += ProductOptionsChanged;
            }
        }

        /// <summary>
        /// Product variants available for this product
        /// </summary>
        [DataMember]
        public ProductVariantCollection ProductVariants
        {
            get { return _productVariants; }
            set
            {
                _productVariants = value;
                _productVariants.CollectionChanged += ProductVariantsChanged;
            }
        }

        /// <summary>
        /// Returns a collection of ProductOption given as list of attributes (choices)
        /// </summary>
        /// <param name="attributes">A collection of <see cref="IProductAttribute"/></param>
        /// <remarks>
        /// This is mainly used for suggesting sku defaults for ProductVariantes
        /// </remarks>
        public IEnumerable<IProductOption> ProductOptionsForAttributes(IEnumerable<IProductAttribute> attributes)
        {
            var options = new List<IProductOption>();
            foreach (var att in attributes)
            {
                options.AddRange(_productOptions.Where(option => OptionContainsAttribute(option, att)));
            }
            return options;
        }


        public static bool OptionContainsAttribute(IProductOption option, IProductAttribute attribute)
        {
            return option.Choices.Any(choice => choice.Id == attribute.Id);
        }

        /// <summary>
        /// Associates a product with a warehouse
        /// </summary>
        /// <param name="warehouseId">The 'unique' id of the <see cref="IWarehouse"/></param>
        public void AddToWarehouse(int warehouseId)
        {
            _variantMaster.AddToWarehouse(warehouseId);
        }

        #endregion

        #region Overrides IProductBase

        internal IProductVariant ProductVariantMaster
        {
            get { return _variantMaster; }
        }

        /// <summary>
        /// Exposes the product variant template's key
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey
        {
            get { return _variantMaster.Key; }
        }

        /// <summary>
        /// Exposes the product variant template's name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _variantMaster.Name; }
            set { _variantMaster.Name = value; }
        }

        /// <summary>
        /// Exposes the product variant template's sku
        /// </summary>
        [DataMember]
        public string Sku
        {
            get { return _variantMaster.Sku; }
            set { _variantMaster.Sku = value; }
        }

        /// <summary>
        /// Exposes the product variant template's price
        /// </summary>
        [DataMember]
        public decimal Price
        {
            get { return _variantMaster.Price; }
            set { _variantMaster.Price = value; }
        }

        /// <summary>
        /// Exposes the product variant template's cost of goods
        /// </summary>
        [DataMember]
        public decimal? CostOfGoods
        {
            get { return _variantMaster.CostOfGoods; }
            set { _variantMaster.CostOfGoods = value; }
        }

        /// <summary>
        /// Exposes the product variant template's sale price
        /// </summary>
        [DataMember]
        public decimal? SalePrice
        {
            get { return _variantMaster.SalePrice; }
            set { _variantMaster.SalePrice = value; }
        }

        /// <summary>
        /// Exposes the product variant template's on sale value
        /// </summary>
        [DataMember]
        public bool OnSale
        {
            get { return _variantMaster.OnSale; }
            set { _variantMaster.OnSale = value; }
        }

        /// <summary>
        /// Exposes the product variant template's weight
        /// </summary>
        [DataMember]
        public decimal? Weight
        {
            get { return _variantMaster.Weight; }
            set { _variantMaster.Weight = value; }
        }

        /// <summary>
        /// Exposes the product variant template's length
        /// </summary>
        [DataMember]
        public decimal? Length
        {
            get { return _variantMaster.Length; }
            set { _variantMaster.Length = value; }
        }

        /// <summary>
        /// Exposes the product variant template's width
        /// </summary>
        [DataMember]
        public decimal? Width
        {
            get { return _variantMaster.Width; }
            set { _variantMaster.Width = value; }
        }

        /// <summary>
        /// Exposes the product variant template's height
        /// </summary>
        [DataMember]
        public decimal? Height
        {
            get { return _variantMaster.Height; }
            set { _variantMaster.Height = value; }
        }

        /// <summary>
        /// Exposes the product variant template's barcode
        /// </summary>
        [DataMember]
        public string Barcode
        {
            get { return _variantMaster.Barcode; }
            set { _variantMaster.Barcode = value; }
        }

        /// <summary>
        /// Exposes the product variant template's available value
        /// </summary>
        [DataMember]
        public bool Available
        {
            get { return _variantMaster.Available; }
            set { _variantMaster.Available = value; }
        }

        /// <summary>
        /// Exposes the product variant template's trackinventory value
        /// </summary>
        [DataMember]
        public bool TrackInventory
        {
            get { return _variantMaster.TrackInventory; }
            set { _variantMaster.TrackInventory = value; }
        }

        /// <summary>
        /// Exposes the product variant template's outofstockpurchase value
        /// </summary>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get { return _variantMaster.OutOfStockPurchase; }
            set { _variantMaster.OutOfStockPurchase = value; }
        }

        /// <summary>
        /// Exposes the product variant template's taxable value
        /// </summary>
        [DataMember]
        public bool Taxable
        {
            get { return _variantMaster.Taxable; }
            set { _variantMaster.Taxable = value; }
        }

        /// <summary>
        /// Exposes the product variant template's shippable value
        /// </summary>
        [DataMember]
        public bool Shippable
        {
            get { return _variantMaster.Shippable; }
            set { _variantMaster.Shippable = value; }
        }

        /// <summary>
        /// Exposes the product variant template's download value
        /// </summary>
        [DataMember]
        public bool Download
        {
            get { return _variantMaster.Download; }
            set { _variantMaster.Download = value; }
        }

        /// <summary>
        /// Exposes the product variant template's downloadmediaid value
        /// </summary>
        [DataMember]
        public int? DownloadMediaId
        {
            get { return _variantMaster.DownloadMediaId; }
            set { _variantMaster.DownloadMediaId = value; }
        }

        /// <summary>
        /// Exposes the product variant template's inventory collection
        /// </summary>
        [DataMember]
        public IEnumerable<IWarehouseInventory> Inventory
        {
            get { return _variantMaster.Inventory; }
        }

        #endregion

        public override void ResetDirtyProperties()
        {
            base.ResetDirtyProperties();
            _variantMaster.ResetDirtyProperties();
        }

        internal override void AddingEntity()
        {
            base.AddingEntity();
            ((ProductVariant) _variantMaster).Master = true;
            ((ProductVariant) _variantMaster).AddingEntity();
        }

        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            ((ProductVariant)_variantMaster).UpdatingEntity();
        }


    }
}