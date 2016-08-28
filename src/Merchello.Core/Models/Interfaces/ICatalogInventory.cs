namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines product inventory for a warehouse
    /// </summary>
    public interface ICatalogInventory
    {
        /// <summary>
        /// Gets the unique key of the product variant
        /// </summary>
        [DataMember]
        Guid ProductVariantKey { get; }

        /// <summary>
        /// Gets the unique catalog key
        /// </summary>
        [DataMember]
        Guid CatalogKey { get; }

        /// <summary>
        /// Gets or sets the number of products in inventory for the warehouse
        /// </summary>
        [DataMember]
        int Count { get; set; }

        /// <summary>
        /// Gets or sets the number at which inventory for the product is considered to be low
        /// </summary>
        [DataMember]
        int LowCount { get; set; }

        /// <summary>
        /// Gets or sets the location of the product.
        /// </summary>
        string Location { get; set; }
        
        /// <summary>
        /// Gets or sets the record update date.
        /// </summary>
        [DataMember]
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the record create date.
        /// </summary>
        [DataMember]
        DateTime CreateDate { get; set; } 
    }
}