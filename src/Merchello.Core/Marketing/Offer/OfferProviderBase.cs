namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// A base class to be implemented by resolved OfferProviders.
    /// </summary>
    /// <typeparam name="TOffer">
    /// The type of the offer to be managed
    /// </typeparam>
    /// <remarks>
    /// This is used in provider resolution
    /// </remarks>
    public abstract class OfferProviderBase<TOffer> : IOfferBaseManager<TOffer>, IOfferProvider
        where TOffer : OfferBase
    {
        /// <summary>
        /// The <see cref="IOfferSettingsService"/>.
        /// </summary>
        private readonly IOfferSettingsService _offerSettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferProviderBase{TOffer}"/> class.
        /// </summary>
        /// <param name="offerSettingsService">
        /// The <see cref="IOfferSettingsService"/>.
        /// </param>
        protected OfferProviderBase(IOfferSettingsService offerSettingsService)
        {
            Mandate.ParameterNotNull(offerSettingsService, "offerSettingsService");

            _offerSettingsService = offerSettingsService;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <remarks>
        /// This should be a unique GUID for each OfferProvider
        /// </remarks>
        public abstract Guid Key { get; }

        #region implementation of IOfferBaseManager

        /// <summary>
        /// Creates a <see cref="IOffer"/> without saving it's settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <returns>
        /// The <see cref="OfferBase"/>.
        /// </returns>
        public virtual TOffer CreateOffer(string name, string offerCode, bool active = true)
        {
            return CreateOffer(name, offerCode, DateTime.MinValue, DateTime.MaxValue, active);
        }

        /// <summary>
        /// Creates a <see cref="IOffer"/> without saving it's settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name of the offer.
        /// </param>
        /// <param name="offerCode">
        /// The offer code
        /// </param>
        /// <param name="offerStartDate">
        /// The start of the offer valid period.
        /// </param>
        /// <param name="offerEndsDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// A value indicating whether or not this offer is active.  Overrides the valid date period.
        /// </param>
        /// <returns>
        /// The <see cref="IOffer"/>.
        /// </returns>
        public virtual TOffer CreateOffer(
            string name,
            string offerCode,
            DateTime offerStartDate,
            DateTime offerEndsDate,
            bool active = true)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(offerCode, "offerCode");

            var settings = _offerSettingsService.CreateOfferSettings(name, offerCode, Key);
            settings.OfferStartsDate = offerStartDate;
            settings.OfferStartsDate = offerEndsDate;
            settings.Active = active;

            return this.GetInstance(settings);
        }

        /// <summary>
        /// Creates a <see cref="IOffer"/> without saving it's settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <returns>
        /// The <see cref="OfferBase"/>.
        /// </returns>
        public virtual TOffer CreateOfferWithKey(string name, string offerCode, bool active = true)
        {
            return CreateOfferWithKey(name, offerCode, DateTime.MinValue, DateTime.MinValue, active);
        }

        /// <summary>
        /// Creates a <see cref="IOffer"/> and saves its settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name of the offer.
        /// </param>
        /// <param name="offerCode">
        /// The offer code
        /// </param>
        /// <param name="offerStartDate">
        /// The start of the offer valid period.
        /// </param>
        /// <param name="offerEndsDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// A value indicating whether or not this offer is active.  Overrides the valid date period.
        /// </param>
        /// <returns>
        /// The <see cref="IOffer"/>.
        /// </returns>
        public virtual TOffer CreateOfferWithKey(
            string name,
            string offerCode,
            DateTime offerStartDate,
            DateTime offerEndsDate,
            bool active = true)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(offerCode, "offerCode");

            var settings = _offerSettingsService.CreateOfferSettingsWithKey(name, offerCode, Key);

            if (!(offerStartDate.Equals(DateTime.MinValue) && offerEndsDate.Equals(DateTime.MaxValue)) || !active)
            {
                settings.OfferStartsDate = offerStartDate;
                settings.OfferStartsDate = offerEndsDate;
                settings.Active = active;
                _offerSettingsService.Save(settings);
            }
            
            return this.GetInstance(settings);
        }

        /// <summary>
        /// Saves the offer
        /// </summary>
        /// <param name="offer">
        /// The offer to be saved
        /// </param>
        public void Save(TOffer offer)
        {
            _offerSettingsService.Save(offer.Settings);
        }

        /// <summary>
        /// Deletes the offer
        /// </summary>
        /// <param name="offer">
        /// The offer to be deleted
        /// </param>
        public void Delete(TOffer offer)
        {
            _offerSettingsService.Delete(offer.Settings);
        }

        /// <summary>
        /// Gets all of the offers managed by this provider
        /// </summary>
        /// <param name="activeOnly">
        /// The active only.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TOffer}"/>.
        /// </returns>
        public IEnumerable<TOffer> GetOffers(bool activeOnly = true)
        {
            var settingsCollection = _offerSettingsService.GetByOfferProviderKey(Key, activeOnly);

            return settingsCollection.Select(GetInstance);
        }

        /// <summary>
        /// The get by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TOffer"/>.
        /// </returns>
        public TOffer GetByKey(Guid key)
        {
            var settings = _offerSettingsService.GetByKey(key);
            return GetInstance(settings);
        }

        #endregion

        /// <summary>
        /// Instantiates an offer given it's settings
        /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <returns>
        /// The <see cref="TOffer"/>.
        /// </returns>
        protected abstract TOffer GetInstance(IOfferSettings offerSettings);
    }
}