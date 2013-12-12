using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines a warehouse catalog
    /// </summary>
    /// <remarks>
    /// 
    /// Warehouses can have mulitple catalogs for inventory purposes.  The idea here is to 
    /// provide a way to seperate out Shipping Methods based on types of products - eg.  If an product 
    /// can only ship overnight and refrigerated (such as Live Lobster) we need to be able to seperate
    /// this out from a T-Shirt.  In this case there could be a Warehouse Catalog with common items and
    /// a separate Warehouse Catalog for Frozen/Live Items.
    /// 
    /// </remarks>
    public interface IWarehouseCatalog : IEntity
    {
        /// <summary>
        /// The unique key identifying the warehouse that maintains this catalog
        /// </summary>
        Guid WarehouseKey { get; }

        /// <summary>
        /// The optional name or title of the catalog
        /// </summary>
        [DataMember]
        [NullSetting(NullSetting = NullSettings.Null)]
        string Name { get; set; }

        /// <summary>
        /// The optional description of the catalog
        /// </summary>
        [DataMember]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        string Description { get; set; }
    }
}