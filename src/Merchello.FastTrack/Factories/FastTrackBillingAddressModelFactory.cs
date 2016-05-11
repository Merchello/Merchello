namespace Merchello.FastTrack.Factories
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.FastTrack.Models;
    using Merchello.Web.Factories;

    using Umbraco.Core;

    /// <summary>
    /// Overrides the default factory settings to use first name and last name to the <see cref="IAddress"/> Name field.
    /// </summary>
    public class FastTrackBillingAddressModelFactory : FastTrackCheckoutAddressModelFactoryBase<FastTrackBillingAddressModel>
    {
        /// <summary>
        /// The <see cref="IStoreSettingService"/>.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackBillingAddressModelFactory"/> class.
        /// </summary>
        public FastTrackBillingAddressModelFactory()
            : this(MerchelloContext.Current.Services.StoreSettingService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackBillingAddressModelFactory"/> class.
        /// </summary>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        public FastTrackBillingAddressModelFactory(IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");
            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Overrides model creation.
        /// </summary>
        /// <param name="address">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="FastTrackBillingAddressModel"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="IAddress"/>.
        /// </returns>
        protected override IAddress OnCreate(IAddress address, FastTrackBillingAddressModel adr)
        {
            
            address.Name = string.Format("{0} {1}", adr.FirstName, adr.Label);

            return base.OnCreate(address, adr);
        }

        /// <summary>
        /// Gets a list of available countries for the billing address.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{SelectListItem}"/>.
        /// </returns>
        protected override IEnumerable<SelectListItem> GetCountrySelectListItems()
        {
            var countries = _storeSettingService.GetAllCountries();
            return GetSelectListItems(countries);
        }
    }
}