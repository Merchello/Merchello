namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Catalog inventory
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class CatalogInventory : ICatalogInventory
    {
        /// <summary>
        /// The catalog key.
        /// </summary>
        private readonly Guid _catalogKey;

        /// <summary>
        /// The product variant key.
        /// </summary>
        private readonly Guid _productVariantKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogInventory"/> class.
        /// </summary>
        /// <param name="catalogKey">
        /// The catalog key.
        /// </param>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        public CatalogInventory(Guid catalogKey, Guid productVariantKey)
        {            
            Mandate.ParameterCondition(catalogKey != Guid.Empty, "catalogKey");
            Mandate.ParameterCondition(productVariantKey != Guid.Empty, "productVariantKey");
            _catalogKey = catalogKey;
            _productVariantKey = productVariantKey;
        }

        /// <summary>
        /// Gets the unique key identifying the warehouse that maintains this catalog
        /// </summary>
        [DataMember]
        public Guid CatalogKey
        {
            get { return _catalogKey; }
        }

        /// <summary>
        /// Gets the unique key of the product variant
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey 
        {
            get { return _productVariantKey; }
        }

        /// <summary>
        /// Gets or sets the number of products in inventory for the warehouse
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the number at which inventory for the product is considered to be low
        /// </summary>
        [DataMember]
        public int LowCount { get; set; }

        /// <summary>
        /// Gets or sets the location of the product.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [DataMember]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [DataMember]
        public DateTime CreateDate { get; set; }
    }
}