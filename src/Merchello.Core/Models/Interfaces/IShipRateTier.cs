namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a shipment rate tier
    /// </summary>
    public interface IShipRateTier : IEntity
    {
        /// <summary>
        /// Gets the 'unique' key of the ship method
        /// </summary>
        [DataMember]
        Guid ShipMethodKey { get; }

        /// <summary>
        /// Gets or sets the low end of the range defined by this tier
        /// </summary>
        [DataMember]
        decimal RangeLow { get; set; }

        /// <summary>
        /// Gets or sets the high end of the range defined by this tier
        /// </summary>
        [DataMember]
        decimal RangeHigh { get; set; }

        /// <summary>
        /// Gets or sets the rate or cost for this range
        /// </summary>
        [DataMember]
        decimal Rate { get; set; }

    }
}