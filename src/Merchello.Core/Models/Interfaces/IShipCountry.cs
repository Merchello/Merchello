namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a country that can be used in shipping rules.
    /// </summary>
    public interface IShipCountry : ICountryBase
    {
        /// <summary>
        /// Gets the warehouse catalog key
        /// </summary>
        [DataMember]
        Guid CatalogKey { get; }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="IShipCountry"/> defines a province collection.
        /// </summary>
        bool HasProvinces { get; }
    }
}