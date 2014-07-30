namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the ShipCountryServcie
    /// </summary>
    public interface IShipCountryService : IService
    {

        //Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, string countryCode, bool raiseEvents = true);
        //Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, ICountry country, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="shipCountry"/>
        /// </summary>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise Events.
        /// </param>
        void Save(IShipCountry shipCountry, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IShipCountry"/> object
        /// </summary>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise Events.
        /// </param>
        void Delete(IShipCountry shipCountry, bool raiseEvents = true);

        /// <summary>
        /// Gets a single <see cref="IShipCountry"/> by it's unique key (Guid pk)
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IShipCountry"/>.
        /// </returns>
        IShipCountry GetByKey(Guid key);

        /// <summary>
        /// Gets a single <see cref="IShipCountry"/>
        /// </summary>
        /// <param name="catalogKey">
        /// The warehouse catalog key (guid)
        /// </param>
        /// <param name="countryCode">
        /// The two letter ISO country code
        /// </param>
        /// <returns>
        /// The <see cref="IShipCountry"/>.
        /// </returns>
        IShipCountry GetShipCountryByCountryCode(Guid catalogKey, string countryCode);

        /// <summary>
        /// Gets a list of <see cref="IShipCountry"/> objects given a <see cref="IWarehouseCatalog"/> key
        /// </summary>
        /// <param name="catalogKey">The catalog key</param>
        /// <returns>A collection of <see cref="IShipCountry"/></returns>
        IEnumerable<IShipCountry> GetShipCountriesByCatalogKey(Guid catalogKey);
    }
}