namespace Merchello.FastTrack.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Web.Store.Models;

    /// <summary>
    /// A base class for FastTrack <see cref="StoreAddressModel"/>.
    /// </summary>
    public class FastTrackCheckoutAddressModel : StoreAddressModel, IFastTrackCheckoutAddressModel
    {
        /// <summary>
        /// Gets or sets the list of countries for the view drop down list.
        /// </summary>
        public IEnumerable<SelectListItem> Countries { get; set; }

        /// <summary>
        /// Gets or sets the success URL to redirect to the shipping entry stage.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }
    }
}