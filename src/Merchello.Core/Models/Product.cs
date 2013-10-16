using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product variant
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Product : KeyEntity, IProduct
    {
        
        private readonly IProductVariant _variant;
        private ProductOptionCollection _productOptions;
        private ProductVariantCollection _productVariants;

        public Product(IProductVariant variant)
            : this(variant, new ProductOptionCollection(), new ProductVariantCollection())
        {}

        public Product(IProductVariant variant, ProductOptionCollection productOptions, ProductVariantCollection productVariants)
        {
            Mandate.ParameterNotNull(variant, "variantMaster");
            Mandate.ParameterNotNull(productOptions, "optionCollection");
            Mandate.ParameterNotNull(productVariants, "productVariants");

            _variant = variant;
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
            _variant.AddToWarehouse(warehouseId);
        }

        /// <summary>
        /// Returns the "master" <see cref="IProductVariant"/> that defines this <see cref="IProduct" /> or null if this <see cref="IProduct" /> has options
        /// </summary>
        /// <returns><see cref="IProductVariant"/> or null if this <see cref="IProduct" /> has options</returns>
        public IProductVariant GetVariantForPurchase()
        {
            if (ProductOptions.Any()) return null;
            return MasterVariant;
        }

        /// <summary>
        /// Returns the <see cref="IProductVariant"/> of this <see cref="IProduct"/> that contains a matching collection of <see cref="IProductAttribute" />. 
        /// If not match is found, returns null.
        /// </summary>
        /// <param name="selectedChoices">A collection of <see cref="IProductAttribute"/> which define the specific <see cref="IProductVariant"/> of the <see cref="IProduct"/></param>
        /// <returns><see cref="IProductVariant"/> or null if no <see cref="IProductVariant"/> is found with a matching collection of <see cref="IProductAttribute"/></returns>
        public IProductVariant GetVariantForPurchase(IEnumerable<IProductAttribute> selectedChoices)
        {
            return
                ProductVariants.FirstOrDefault(
                    variant =>
                    {
                        var productAttributes = selectedChoices as IProductAttribute[] ?? selectedChoices.ToArray();
                        return variant.Attributes.Count() == productAttributes.Count() &&
                                          productAttributes.All(item => ((ProductAttributeCollection)variant.Attributes).Contains(item.Id));
                    });

        }

        /// <summary>
        /// Returns the <see cref="IProductVariant"/> of this <see cref="IProduct"/> that contains a matching collection of <see cref="IProductAttribute" />. 
        /// If not match is found, returns null.
        /// </summary>
        /// <param name="selectedChoiceIds"></param>
        /// <returns><see cref="IProductVariant"/> or null if no <see cref="IProductVariant"/> is found with a matching collection of <see cref="IProductAttribute"/></returns>
        public IProductVariant GetVariantForPurchase(int[] selectedChoiceIds)
        {
            return
                ProductVariants.FirstOrDefault(
                    variant => variant.Attributes.Count() == selectedChoiceIds.Length &&
                               selectedChoiceIds.All(id => ((ProductAttributeCollection) variant.Attributes).Contains(id)));
        }

        #endregion

        #region Overrides IProductBase

        internal IProductVariant MasterVariant
        {
            get { return _variant; }
        }

        /// <summary>
        /// Exposes the product variant template's key
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey
        {
            get { return _variant.Key; }
        }

        /// <summary>
        /// Exposes the product variant template's name
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _variant.Name; }
            set { _variant.Name = value; }
        }

        /// <summary>
        /// Exposes the product variant template's sku
        /// </summary>
        [DataMember]
        public string Sku
        {
            get { return _variant.Sku; }
            set { _variant.Sku = value; }
        }

        /// <summary>
        /// Exposes the product variant template's price
        /// </summary>
        [DataMember]
        public decimal Price
        {
            get { return _variant.Price; }
            set { _variant.Price = value; }
        }

        /// <summary>
        /// Exposes the product variant template's cost of goods
        /// </summary>
        [DataMember]
        public decimal? CostOfGoods
        {
            get { return _variant.CostOfGoods; }
            set { _variant.CostOfGoods = value; }
        }

        /// <summary>
        /// Exposes the product variant template's sale price
        /// </summary>
        [DataMember]
        public decimal? SalePrice
        {
            get { return _variant.SalePrice; }
            set { _variant.SalePrice = value; }
        }

        /// <summary>
        /// Exposes the product variant template's on sale value
        /// </summary>
        [DataMember]
        public bool OnSale
        {
            get { return _variant.OnSale; }
            set { _variant.OnSale = value; }
        }

        /// <summary>
        /// Exposes the product variant template's weight
        /// </summary>
        [DataMember]
        public decimal? Weight
        {
            get { return _variant.Weight; }
            set { _variant.Weight = value; }
        }

        /// <summary>
        /// Exposes the product variant template's length
        /// </summary>
        [DataMember]
        public decimal? Length
        {
            get { return _variant.Length; }
            set { _variant.Length = value; }
        }

        /// <summary>
        /// Exposes the product variant template's width
        /// </summary>
        [DataMember]
        public decimal? Width
        {
            get { return _variant.Width; }
            set { _variant.Width = value; }
        }

        /// <summary>
        /// Exposes the product variant template's height
        /// </summary>
        [DataMember]
        public decimal? Height
        {
            get { return _variant.Height; }
            set { _variant.Height = value; }
        }

        /// <summary>
        /// Exposes the product variant template's barcode
        /// </summary>
        [DataMember]
        public string Barcode
        {
            get { return _variant.Barcode; }
            set { _variant.Barcode = value; }
        }

        /// <summary>
        /// Exposes the product variant template's available value
        /// </summary>
        [DataMember]
        public bool Available
        {
            get { return _variant.Available; }
            set { _variant.Available = value; }
        }

        /// <summary>
        /// Exposes the product variant template's trackinventory value
        /// </summary>
        [DataMember]
        public bool TrackInventory
        {
            get { return _variant.TrackInventory; }
            set { _variant.TrackInventory = value; }
        }

        /// <summary>
        /// Exposes the product variant template's outofstockpurchase value
        /// </summary>
        [DataMember]
        public bool OutOfStockPurchase
        {
            get { return _variant.OutOfStockPurchase; }
            set { _variant.OutOfStockPurchase = value; }
        }

        /// <summary>
        /// Exposes the product variant template's taxable value
        /// </summary>
        [DataMember]
        public bool Taxable
        {
            get { return _variant.Taxable; }
            set { _variant.Taxable = value; }
        }

        /// <summary>
        /// Exposes the product variant template's shippable value
        /// </summary>
        [DataMember]
        public bool Shippable
        {
            get { return _variant.Shippable; }
            set { _variant.Shippable = value; }
        }

        /// <summary>
        /// Exposes the product variant template's download value
        /// </summary>
        [DataMember]
        public bool Download
        {
            get { return _variant.Download; }
            set { _variant.Download = value; }
        }

        /// <summary>
        /// Exposes the product variant template's downloadmediaid value
        /// </summary>
        [DataMember]
        public int? DownloadMediaId
        {
            get { return _variant.DownloadMediaId; }
            set { _variant.DownloadMediaId = value; }
        }

        /// <summary>
        /// Exposes the product variant template's inventory collection
        /// </summary>
        [DataMember]
        public IEnumerable<IWarehouseInventory> Warehouses
        {
            get { return _variant.Warehouses; }
        }

        #endregion

        public override void ResetDirtyProperties()
        {
            base.ResetDirtyProperties();
            _variant.ResetDirtyProperties();
        }

        internal override void AddingEntity()
        {
            base.AddingEntity();
            ((ProductVariant) _variant).Master = true;
            ((ProductVariant) _variant).AddingEntity();
        }

        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            ((ProductVariant)_variant).UpdatingEntity();
        }


    }
}