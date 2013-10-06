using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Tests.Base.Prototyping.Models
{
    /// <summary>
    /// Defines product inventory for a warehouse
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// The warehouse Id
        /// </summary>
        [DataMember]
        int WarehouseId { get; }

        /// <summary>
        /// The unique key of the product variant
        /// </summary>
        [DataMember]
        Guid ProductVariantKey { get; }

        /// <summary>
        /// The number of products in inventory for the warehouse
        /// </summary>
        [DataMember]
        int Count { get; set;  }

        /// <summary>
        /// The number at which inventory for the product is considered to be low
        /// </summary>
        [DataMember]
        int LowCount { get; set; }

        /// <summary>
        /// The date the inventory record was last updated
        /// </summary>
        [DataMember]
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// The date the inventory record was created
        /// </summary>
        [DataMember]
        DateTime CreateDate { get; set; }
    }
}