namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines an offer settings service
    /// </summary>
    public interface IOfferSettingsService : IPageCachedService<IOfferSettings>
    {
        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        IOfferSettings CreateOfferSettings(string name, string offerCode, Guid offerProviderKey, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="componentDefinitions">
        /// The component definitions.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        IOfferSettings CreateOfferSettings(string name, string offerCode, Guid offerProviderKey, OfferComponentDefinitionCollection componentDefinitions, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> and saves it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        IOfferSettings CreateOfferSettingsWithKey(string name, string offerCode, Guid offerProviderKey, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> and saves it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="componentDefinitions">
        /// The component definitions.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        IOfferSettings CreateOfferSettingsWithKey(string name, string offerCode, Guid offerProviderKey, OfferComponentDefinitionCollection componentDefinitions, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IOfferSettings offerSettings, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offersSettings">
        /// The offers settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IEnumerable<IOfferSettings> offersSettings, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IOfferSettings offerSettings, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offersSettings">
        /// The offers settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Delete(IEnumerable<IOfferSettings> offersSettings, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IOfferSettings"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        IOfferSettings GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IOfferSettings"/> by their unique keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings}"/>.
        /// </returns>
        IEnumerable<IOfferSettings> GetByKeys(IEnumerable<Guid> keys); 
            
        /// <summary>
        /// Gets a collection of <see cref="IOfferSettings"/> for a given offer provider.
        /// </summary>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="activeOnly">
        /// Optional value indicating whether or not to only return active Offers settings marked as active
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings}"/>.
        /// </returns>
        IEnumerable<IOfferSettings> GetByOfferProviderKey(Guid offerProviderKey, bool activeOnly = true);
        
        /// <summary>
        /// Gets a <see cref="OfferSettings"/> by the offer code value.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        IOfferSettings GetByOfferCode(string offerCode);

        /// <summary>
        /// Gets a collection of active <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="excludeExpired">
        /// The exclude Expired.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings"/>.
        /// </returns>
        IEnumerable<IOfferSettings> GetAllActive(bool excludeExpired = true);

        /// <summary>
        /// Checks if the offer code is unique.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// A valid indicating whether or not the offer code is unique.
        /// </returns>
        bool OfferCodeIsUnique(string offerCode);

        Page<IOfferSettings> GetPage(string filterTerm, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);
    }
}