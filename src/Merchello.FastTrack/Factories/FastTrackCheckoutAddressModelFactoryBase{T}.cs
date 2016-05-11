namespace Merchello.FastTrack.Factories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core.Models;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A base class for building FastTrack checkout address models.
    /// </summary>
    /// <typeparam name="TAddress">
    /// The type of <see cref="CheckoutAddressModel"/>
    /// </typeparam>
    public abstract class FastTrackCheckoutAddressModelFactoryBase<TAddress> : CheckoutAddressModelFactory<TAddress>
        where TAddress : class, ICheckoutAddressModel, new()
    {
        /// <summary>
        /// Gets a list of available countries for the respective address.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{SelectListItem}"/>.
        /// </returns>
        protected abstract IEnumerable<SelectListItem> GetCountrySelectListItems();

        /// <summary>
        /// Maps a collection of <see cref="ICountry"/> to a list of <see cref="SelectListItem"/>.
        /// </summary>
        /// <param name="countries">
        /// The countries.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SelectListItem}"/>.
        /// </returns>
        protected IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<ICountry> countries)
        {
            return countries.Select(x => new SelectListItem { Value = x.CountryCode, Text = x.Name });
        }
    }
}