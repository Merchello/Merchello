namespace Merchello.FastTrack.Factories
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Core;
    using Core.Models;
    using Merchello.Core.Services;
    using Merchello.FastTrack.Models;

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
            Ensure.ParameterNotNull(storeSettingService, "storeSettingService");
            _storeSettingService = storeSettingService;
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

        /// <summary>
        /// Overrides the creation of the <see cref="FastTrackBillingAddressModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="FastTrackBillingAddressModel"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="FastTrackBillingAddressModel"/>.
        /// </returns>
        protected override FastTrackBillingAddressModel OnCreate(FastTrackBillingAddressModel model, IAddress adr)
        {
            model.UseForShipping = true;
            return base.OnCreate(model, adr);
        }

        /// <summary>
        /// Overrides the creation of <see cref="ICustomerAddress"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICustomerAddress"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="FastTrackBillingAddressModel"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomer"/>.
        /// </param>
        /// <param name="label">
        /// The customer address label (e.g. Billing Address).
        /// </param>
        /// <param name="addressType">
        /// The <see cref="AddressType"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICustomerAddress"/>.
        /// </returns>
        protected override ICustomerAddress OnCreate(ICustomerAddress model, FastTrackBillingAddressModel adr, ICustomer customer, string label, AddressType addressType)
        {
            // Set the address to the default address
            model.IsDefault = true;

            return base.OnCreate(model, adr, customer, label, addressType);
        }
    }
}