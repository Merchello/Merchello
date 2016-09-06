namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a shipping country.
    /// </summary>
    public interface IShipCountry : ICountryBase
    {
        /// <summary>
        /// Gets the unique warehouse catalog key
        /// </summary>
        [DataMember]
        Guid CatalogKey { get; }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="IShipCountry"/> defines a province collection.
        /// </summary>
        bool HasProvinces { get; }
    }
}