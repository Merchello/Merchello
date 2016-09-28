namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a shipment rate tier
    /// </summary>
    public interface IShipRateTier : IEntity
    {
        /// <summary>
        /// Gets the key of the ship method
        /// </summary>
        Guid ShipMethodKey { get; }

        /// <summary>
        /// Gets or sets low end of the range defined by this tier
        /// </summary>
        decimal RangeLow { get; set; }

        /// <summary>
        /// Gets or sets the high end of the range defined by this tier
        /// </summary>
        decimal RangeHigh { get; set; }

        /// <summary>
        /// Gets or sets the rate or cost for this range
        /// </summary>
        decimal Rate { get; set; }
    }
}