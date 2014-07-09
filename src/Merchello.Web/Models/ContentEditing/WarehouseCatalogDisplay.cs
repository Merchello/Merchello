namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The warehouse catalog display.
    /// </summary>
    public class WarehouseCatalogDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the warehouse key.
        /// </summary>
        public Guid WarehouseKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}
