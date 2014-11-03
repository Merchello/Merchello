namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// The shipment status display.
    /// </summary>
    public class ShipmentStatusDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reportable.
        /// </summary>
        public bool Reportable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the status is an active status.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; } 
    }

    /// <summary>
    /// The shipment status display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ShipmentStatusDisplayExtensions
    {
        /// <summary>
        /// Maps a <see cref="IShipmentStatus"/> to a <see cref="ShipmentStatusDisplay"/>
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="ShipmentStatusDisplay"/>.
        /// </returns>
        public static ShipmentStatusDisplay ToShipmentStatusDisplay(this IShipmentStatus status)
        {
            return AutoMapper.Mapper.Map<ShipmentStatusDisplay>(status);
        }
    }
}