namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// A base class to be implemented by resolved OfferProviders.
    /// </summary>
    /// <remarks>
    /// This is used in provider resolution
    /// </remarks>
    public abstract class OfferProviderBase : IOfferProvider
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

        public OfferBase CreateOffer(
            string name,
            string offerCode,
            DateTime offerStartDate,
            DateTime offerExpiresDate,
            bool active = true)
        {
            throw new NotImplementedException();
        }

        public OfferBase CreateOfferWithKey(
            string name,
            string offerCode,
            DateTime offerStartDate,
            DateTime offerExpiresDate,
            bool active = true)
        {
            throw new NotImplementedException();
        }

        public void Save(OfferBase offer)
        {
            throw new NotImplementedException();
        }

        public void Delete(OfferBase offer)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OfferBase> GetOffers(bool activeOnly = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetOffers<T>(bool activeOnly = true) where T : OfferBase
        {
            throw new NotImplementedException();
        }

        public OfferBase GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public T GetByKey<T>(Guid key)
        {
            throw new NotImplementedException();
        }
    }
}