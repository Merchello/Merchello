namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    using NodaMoney;

    /// <summary>
    /// Represents a shipment rate tier.
    /// </summary>
    public interface IShipRateTier : IEntity
    {
        /// <summary>
        /// Gets the 'unique' key of the ship method
        /// </summary>
        
        Guid ShipMethodKey { get; }

        /// <summary>
        /// Gets or sets the low end of the range defined by this tier
        /// </summary>
        
        Money RangeLow { get; set; }

        /// <summary>
        /// Gets or sets the high end of the range defined by this tier
        /// </summary>
        
        Money RangeHigh { get; set; }

        /// <summary>
        /// Gets or sets the rate or cost for this range
        /// </summary>
        
        Money Rate { get; set; }
    }
}