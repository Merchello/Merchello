namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Core.Models;

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

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// The warehouse catalog display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class WarehouseCatalogDisplayExtensions
    {
        /// <summary>
        /// Maps a <see cref="IWarehouseCatalog"/> to a <see cref="WarehouseCatalogDisplay"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseCatalogDisplay"/>.
        /// </returns>
        internal static WarehouseCatalogDisplay ToWarehouseCatalogDisplay(this IWarehouseCatalog warehouseCatalog)
        {
            return AutoMapper.Mapper.Map<WarehouseCatalogDisplay>(warehouseCatalog);
        }

        /// <summary>
        /// Maps a <see cref="WarehouseCatalogDisplay"/> to a <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="catalog">
        /// The catalog.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseCatalog"/>.
        /// </returns>
        internal static IWarehouseCatalog ToWarehouseCatalog(this WarehouseCatalogDisplay catalog, IWarehouseCatalog destination)
        {
            if (!catalog.Key.Equals(Guid.Empty)) destination.Key = catalog.Key;

            destination.Name = catalog.Name;
            destination.Description = catalog.Description;

            return destination;
        }
    }
}
