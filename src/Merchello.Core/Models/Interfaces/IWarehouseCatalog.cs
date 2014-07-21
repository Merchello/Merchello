namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// Defines a warehouse catalog
    /// </summary>
    /// <remarks>
    /// 
    /// Warehouses can have multiple catalogs for inventory purposes.  The idea here is to 
    /// provide a way to separate out Shipping Methods based on types of product.  If an product 
    /// can only ship overnight and refrigerated (such as Live Lobster) we need to be able to separate
    /// this out from a T-Shirt.  In this case there could be a Warehouse Catalog with common items and
    /// a separate Warehouse Catalog for Frozen/Live Items.
    /// 
    /// </remarks>
    public interface IWarehouseCatalog : IEntity
    {
        /// <summary>
        /// Gets the unique key identifying the warehouse that maintains this catalog
        /// </summary>
        Guid WarehouseKey { get; }

        /// <summary>
        /// Gets or sets the optional name or title of the catalog
        /// </summary>
        [DataMember]
        [NullSetting(NullSetting = NullSettings.Null)]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional description of the catalog
        /// </summary>
        [DataMember]
        [Length(500)]
        [NullSetting(NullSetting = NullSettings.Null)]
        string Description { get; set; }
    }
}