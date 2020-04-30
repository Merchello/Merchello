namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Exceptions;
    using Merchello.Core.Models;
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
    public abstract class OfferManagerBase<TOffer> : IOfferManagerBase<TOffer>, IOfferProvider
        where TOffer : OfferBase
    {
        /// <summary>
        /// The <see cref="IOfferSettingsService"/>.
        /// </summary>
        private readonly IOfferSettingsService _offerSettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferManagerBase{TOffer}"/> class.
        /// </summary>
        /// <param name="offerSettingsService">
        /// The <see cref="IOfferSettingsService"/>.
        /// </param>
        protected OfferManagerBase(IOfferSettingsService offerSettingsService)
        {
            Ensure.ParameterNotNull(offerSettingsService, "offerSettingsService");

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

        #region implementation of IOfferManagerBase       

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
        /// Gets a collection of <see cref="TOffer"/> by their unique keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TOffer}"/>.
        /// </returns>
        public IEnumerable<TOffer> GetByKeys(IEnumerable<Guid> keys)
        {
            var settings = _offerSettingsService.GetByKeys(keys);
            return settings.Select(this.GetInstance);
        }

        /// <summary>
        /// Gets an offer by it's offer code (with manager defaults).
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public abstract Attempt<TOffer> GetByOfferCode(string offerCode, ICustomerBase customer);
        

        /// <summary>
        /// Gets an offer by it's offer code.
        /// </summary>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of award
        /// </typeparam>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="customer">
        /// The customer
        /// </param>
        /// <returns>
        /// The <see cref="TOffer"/>.
        /// </returns>
        public Attempt<TOffer> GetByOfferCode<TConstraint, TAward>(string offerCode, ICustomerBase customer)
            where TConstraint : class 
            where TAward : class
        {
            if (string.IsNullOrEmpty(offerCode)) return Attempt<TOffer>.Fail(new OfferRedemptionException("Offer code was not provided"));

            var settings = _offerSettingsService.GetByOfferCode(offerCode);

            if (settings == null) return Attempt<TOffer>.Fail(new OfferRedemptionException("Offer not found by offer code."));

            var instance = this.GetInstance(settings);

            var ensure = instance.EnsureOfferIsValid<TConstraint, TAward>(customer);

            return !ensure.Success ? Attempt<TOffer>.Fail(ensure.Exception) : Attempt<TOffer>.Succeed(instance);
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