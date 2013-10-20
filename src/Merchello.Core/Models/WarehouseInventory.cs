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
        private readonly int _warehouseId;
        private readonly int _productVariantId;

        public WarehouseInventory(int warehouseId, int productVariantId)
        {            
            _warehouseId = warehouseId;
            _productVariantId = productVariantId;
        }

        /// <summary>
        /// The warehouse Id
        /// </summary>
        [DataMember]
        public int WarehouseId
        {
            get { return _warehouseId; }
        }

        /// <summary>
        /// The unique id of the product variant
        /// </summary>
        [DataMember]
        public int ProductVariantId {
            get { return _productVariantId; }
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