namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a shipping country.
    /// </summary>
    public interface IShipCountry : ICountry, IEntity
    {
        /// <summary>
        /// Gets the unique warehouse catalog key
        /// </summary>
        
        Guid CatalogKey { get; }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="IShipCountry"/> defines a province collection.
        /// </summary>
        bool HasProvinces { get; }
    }
}