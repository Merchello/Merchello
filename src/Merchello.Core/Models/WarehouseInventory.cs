using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Product variant inventory
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class WarehouseInventory : IWarehouseInventory
    {
        private readonly Guid _warehouseKey;
        private readonly Guid _productVariantKey;

        public WarehouseInventory(Guid warehouseKey, Guid productVariantKey)
        {            
            _warehouseKey = warehouseKey;
            _productVariantKey = productVariantKey;
        }

        /// <summary>
        /// The warehouse key
        /// </summary>
        [DataMember]
        public Guid WarehouseKey
        {
            get { return _warehouseKey; }
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