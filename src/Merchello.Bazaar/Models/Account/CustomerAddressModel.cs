namespace Merchello.Bazaar.Models.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// The customer address model.
    /// </summary>
    public class CustomerAddressModel
    {
        public string Theme { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        [Required]
        public string AddressType { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [Required]
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        [Required(ErrorMessage = "Label is required"), Display(Name = "Address Label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        [Required(ErrorMessage = "Full name is required"), Display(Name = "Full name")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [Required(ErrorMessage = "Label is required"), Display(Name = "Address Label")]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required(ErrorMessage = "Locality is required"), Display(Name = "Locality or city")]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [Required(ErrorMessage = "Region is required"), Display(Name = "Region, State or Provice")]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public IEnumerable<SelectListItem> Regions { get; set; } 

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required(ErrorMessage = "Postal code is required"), Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [Required]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        public IEnumerable<SelectListItem> ShipCountries { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        public IEnumerable<SelectListItem> AllCountries { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}