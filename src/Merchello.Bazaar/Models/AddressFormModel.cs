namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// A site address model.
    /// </summary>
    public class AddressFormModel : IAddress
    {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "Name is reqired"), Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [Required(ErrorMessage = "Address 1 is required"), Display(Name = "Address 1")]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required(ErrorMessage = "Locality is required"), Display(Name = "Locality")]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [Display(Name = "Region")]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required(ErrorMessage = "Postal code is required"), Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; }

            /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [Display(Name = "Country")]
        public IEnumerable<SelectListItem> Countries { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessage = "Email address is required"), Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        [Display(Name = "Organization or Company")]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is commercial.
        /// </summary>
        /// <remarks>
        /// We're not going to use this at the moment
        /// </remarks>
        public bool IsCommercial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether billing is shipping.
        /// </summary>
        [Display(Name = "Ship to this address")]
        public bool BillingIsShipping { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AddressType"/>.
        /// </summary>
        public AddressType AddressType { get; set; }
    }
}