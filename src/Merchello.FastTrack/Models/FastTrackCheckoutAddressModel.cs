namespace Merchello.FastTrack.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Merchello.Web.Store.Localization;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A base class for FastTrack <see cref="CheckoutAddressModel"/>.
    /// </summary>
    public class FastTrackCheckoutAddressModel : CheckoutAddressModel, IFastTrackCheckoutAddressModel
    {
        /// <summary>
        /// Gets or sets the list of countries for the view drop down list.
        /// </summary>
        public IEnumerable<SelectListItem> Countries { get; set; } 
    }
}