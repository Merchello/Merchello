namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Catalog inventory
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class CatalogInventory : ICatalogInventory, IDateStamped
    {
        /// <summary>
        /// The catalog key.
        /// </summary>
        private readonly Guid _catalogKey;

        /// <summary>
        /// The product variant key.
        /// </summary>
        private Guid _productVariantKey;

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
            Ensure.ParameterCondition(catalogKey != Guid.Empty, "catalogKey");

            _catalogKey = catalogKey;
            _productVariantKey = productVariantKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid CatalogKey
        {
            get { return _catalogKey; }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid ProductVariantKey 
        {
            get
            {
                return _productVariantKey;
            }

            internal set
            {
                _productVariantKey = value;
            }
        }

        /// <inheritdoc/>>
        [DataMember]
        public int Count { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public int LowCount { get; set; }

        /// <inheritdoc/>
        public string Location { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        [DataMember]
        public DateTime CreateDate { get; set; }
    }
}