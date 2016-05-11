namespace Merchello.FastTrack.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A base class for FastTrack <see cref="ICheckoutAddressModel"/>.
    /// </summary>
    public interface IFastTrackCheckoutAddressModel : ICheckoutAddressModel
    {
        /// <summary>
        /// Gets or sets the list of countries for the view drop down list.
        /// </summary>
        IEnumerable<SelectListItem> Countries { get; set; }
    }
}