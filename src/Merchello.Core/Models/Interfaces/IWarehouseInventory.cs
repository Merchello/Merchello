using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines product inventory for a warehouse
    /// </summary>
    public interface IWarehouseInventory
    {
        ///// <summary>
        ///// The catalog key
        ///// </summary>
        //[DataMember]
        //Guid CatalogKey { get; }

        /// <summary>
        /// The unique key of the product variant
        /// </summary>
        [DataMember]
        Guid ProductVariantKey { get; }

        /// <summary>
        /// The unique warehouse key
        /// </summary>
        [DataMember]
        Guid WarehouseKey { get; }

        ///// <summary>
        ///// The name of the warehouse catalog
        ///// </summary>
        //[DataMember]
        //string CatalogName { get; }

        /// <summary>
        /// The number of products in inventory for the warehouse
        /// </summary>
        [DataMember]
        int Count { get; set; }
        /// <summary>
        /// The number at which inventory for the product is considered to be low
        /// </summary>
        [DataMember]
        int LowCount { get; set; }

        [DataMember]
        DateTime UpdateDate { get; set; }

        [DataMember]
        DateTime CreateDate { get; set; } 
    }
}