namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchCatalogInventory" table.
    /// </summary>
    internal class CatalogInventoryDto : IDto
    {
        /// <summary>
        /// Gets or sets the catalog key.
        /// </summary>
        public Guid CatalogKey { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the low count.
        /// </summary>
        public int LowCount { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the warehouse catalog dto.
        /// </summary>
        public WarehouseCatalogDto WarehouseCatalogDto { get; set; }

        ///// <summary>
        ///// Gets or sets the product variant dto.
        ///// </summary>
        //[ResultColumn]
        //[Reference(ReferenceType.Foreign, ColumnName = "pk", ReferenceMemberName = "Key")]
        //public ProductVariantDto ProductVariantDto { get; set; } 
    }
}