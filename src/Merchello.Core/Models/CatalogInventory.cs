using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Product variant inventory
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class CatalogInventory : ICatalogInventory
    {
        private readonly IWarehouseCatalog _catalog;
        private readonly Guid _productVariantKey;

        public CatalogInventory(IWarehouseCatalog catalog, Guid productVariantKey)
        {            
            Mandate.ParameterNotNull(catalog, "catalog");
            Mandate.ParameterCondition(productVariantKey != Guid.Empty, "productVariantKey");
            _catalog = catalog;
            _productVariantKey = productVariantKey;
        }

        /// <summary>
        /// The unique key identifying the warehouse that maintains this catalog
        /// </summary>
        [DataMember]
        public Guid CatalogKey
        {
            get { return _catalog.Key; }
        }

        /// <summary>
        /// The optional name or title of the catalog
        /// </summary>
        [IgnoreDataMember]
        internal string CatalogName 
        {
            get { return _catalog.Name; }
        }     

        /// <summary>
        /// The unique key of the product variant
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey {
            get { return _productVariantKey; }
        }

        /// <summary>
        /// The number of products in inventory for the warehouse
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// The number at which inventory for the product is considered to be low
        /// </summary>
        [DataMember]
        public int LowCount { get; set; }

        [DataMember]
        public DateTime UpdateDate { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }
    }
}