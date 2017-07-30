namespace Merchello.Core.Models
{
    using System;
    

    /// <summary>
    /// Represents product inventory for a warehouse catalog.
    /// </summary>
    public interface ICatalogInventory
    {
        /// <summary>
        /// Gets the unique key of the product variant
        /// </summary>
        
        Guid ProductVariantKey { get; }

        /// <summary>
        /// Gets the unique catalog key
        /// </summary>
        
        Guid CatalogKey { get; }

        /// <summary>
        /// Gets or sets the number of products in inventory for the warehouse
        /// </summary>
        
        int Count { get; set; }

        /// <summary>
        /// Gets or sets the number at which inventory for the product is considered to be low
        /// </summary>
        
        int LowCount { get; set; }

        /// <summary>
        /// Gets or sets the location of the product.
        /// </summary>
        string Location { get; set; }
        
        /// <summary>
        /// Gets or sets the record update date.
        /// </summary>
        
        DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the record create date.
        /// </summary>
        
        DateTime CreateDate { get; set; } 
    }
}