namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Core.Models;

    /// <summary>
    /// The catalog inventory display.
    /// </summary>
    public class CatalogInventoryDisplay
    {
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the catalog key.
        /// </summary>
        public Guid CatalogKey { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the low count.
        /// </summary>
        public int LowCount { get; set; }

        /// <summary>
        /// Gets or sets the location of the item.  Intended for shelving or warehouse section labels.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }


    /// <summary>
    /// The catalog inventory display extensions responsible for mapping between <see cref="ICatalogInventory"/> and <see cref="CatalogInventoryDisplay"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CatalogInventoryDisplayExtensions
    {
        #region ICatalogInventory

        /// <summary>
        /// Maps <see cref="CatalogInventoryDisplay"/> to <see cref="ICatalogInventory"/>.
        /// </summary>
        /// <param name="catalogInventoryDisplay">
        /// The catalog inventory display.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="ICatalogInventory"/>.
        /// </returns>
        internal static ICatalogInventory ToCatalogInventory(this CatalogInventoryDisplay catalogInventoryDisplay, ICatalogInventory destination)
        {
            destination.Count = catalogInventoryDisplay.Count;
            destination.LowCount = catalogInventoryDisplay.LowCount;
            destination.Location = catalogInventoryDisplay.Location;

            return destination;
        }

        #endregion
    }
}