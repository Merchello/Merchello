namespace Merchello.FastTrack.Factories
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.FastTrack.Models;
    using Merchello.Web.Factories;

    using Umbraco.Core;

    /// <summary>
    /// The factory responsible for building <see cref="FastTrackCheckoutAddressModel"/>s.
    /// </summary>
    public class FastTrackShippingAddressModelFactory : FastTrackCheckoutAddressModelFactoryBase<FastTrackCheckoutAddressModel>
    {
        /// <summary>
        /// The <see cref="IShippingContext"/>.
        /// </summary>
        private readonly IShippingContext _shippingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackShippingAddressModelFactory"/> class.
        /// </summary>
        public FastTrackShippingAddressModelFactory()
            : this(MerchelloContext.Current.Gateways.Shipping)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastTrackShippingAddressModelFactory"/> class.
        /// </summary>
        /// <param name="shippingContext">
        /// The <see cref="IShippingContext"/>.
        /// </param>
        public FastTrackShippingAddressModelFactory(IShippingContext shippingContext)
        {
            Ensure.ParameterNotNull(shippingContext, "shippingContext");
            _shippingContext = shippingContext;
        }

        /// <summary>
        /// Gets a list of available countries for the shipping address.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{SelectListItem}"/>.
        /// </returns>
        protected override IEnumerable<SelectListItem> GetCountrySelectListItems()
        {
            var countries = _shippingContext.GetAllowedShipmentDestinationCountries();
            return GetSelectListItems(countries);
        }
    }
}