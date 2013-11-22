using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    public interface IWarehouseCountry : ICountryBase
    {
        /// <summary>
        /// The unique warehouse key (guid)
        /// </summary>
        [DataMember]
        Guid WarehouseKey { get; }

        /// <summary>
        /// The ship methods associated with the country
        /// </summary>
        [DataMember]
        ShipMethodCollection ShipMethods { get; }
    }
}