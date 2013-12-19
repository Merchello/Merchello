using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    public interface IShipCountry : ICountryBase
    {
        /// <summary>
        /// The unique warehouse catalog key (guid)
        /// </summary>
        [DataMember]
        Guid CatalogKey { get; }

    }
}