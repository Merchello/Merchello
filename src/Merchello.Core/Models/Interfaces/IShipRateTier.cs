using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a shipment rate tier
    /// </summary>
    public interface IShipRateTier : IEntity
    {
        /// <summary>
        /// The 'unique' key of the ship method
        /// </summary>
        [DataMember]
        Guid ShipMethodKey { get; }

        /// <summary>
        /// The low end of the range defined by this tier
        /// </summary>
        [DataMember]
        decimal RangeLow { get; set; }

        /// <summary>
        /// The high end of the range defined by this tier
        /// </summary>
        [DataMember]
        decimal RangeHigh { get; set; }

        /// <summary>
        /// The rate or cost for this range
        /// </summary>
        [DataMember]
        decimal Rate { get; set; }

    }
}