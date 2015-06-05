namespace Merchello.Core.Marketing.Offer
{
    using System;

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

        /// <summary>
        /// Gets the type name of type managed by this provider.
        /// </summary>
        /// <remarks>
        /// This is used by the UI when determining what restricted offer components (if any) can be assigned
        /// </remarks>
        public virtual string ManagesTypeName
        {
            get
            {
                return typeof(TOffer).Name;
            } 
        }

        #region implementation of IOfferBaseManager       

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

        /// <summary>
        /// Gets an offer by it's offer code.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="TOffer"/>.
        /// </returns>
        public TOffer GetByOfferCode(string offerCode)
        {
            var settings = _offerSettingsService.GetByOfferCode(offerCode);
            return this.GetInstance(settings);
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