namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using Umbraco.Core.Models;

    /// <summary>
    /// The checkout address form.
    /// </summary>
    public class CheckoutAddressForm
    {
        /// <summary>
        /// Gets or sets the theme name.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets the sale summary.
        /// </summary>
        public SalePreparationSummary SaleSummary { get; set; }

        #region Billing address

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "Name is reqired"), Display(Name = "Name")]
        public string BillingName { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [Required(ErrorMessage = "Address 1 is required"), Display(Name = "Address 1")]
        public string BillingAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [Display(Name = "Address 2")]
        public string BillingAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required(ErrorMessage = "Locality is required"), Display(Name = "Locality")]
        public string BillingLocality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [Display(Name = "Region")]
        public string BillingRegion { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required(ErrorMessage = "Postal code is required"), Display(Name = "Postal code")]
        public string BillingPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string BillingCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [Display(Name = "Country")]
        public IEnumerable<SelectListItem> BillingCountries { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        [Display(Name = "Phone")]
        public string BillingPhone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessage = "Email address is required"), Display(Name = "Email Address")]
        [EmailAddress]
        public string BillingEmail { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        [Display(Name = "Organization or Company")]
        public string BillingOrganization { get; set; }

        #endregion

        #region Shipping address

        /// <summary>
        /// Gets or sets a value indicating whether billing is shipping.
        /// </summary>
        public bool BillingIsShipping { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "Name is reqired"), Display(Name = "Name")]
        public string ShippingName { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [Required(ErrorMessage = "Address 1 is required"), Display(Name = "Address 1")]
        public string ShippingAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [Display(Name = "Address 2")]
        public string ShippingAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required(ErrorMessage = "Locality is required"), Display(Name = "Locality")]
        public string ShippingLocality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [Display(Name = "Region")]
        public string ShippingRegion { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required(ErrorMessage = "Postal code is required"), Display(Name = "Postal code")]
        public string ShippingPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string ShippingCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [Display(Name = "Country")]
        public IEnumerable<SelectListItem> ShippingCountries { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        [Display(Name = "Phone")]
        public string ShippingPhone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessage = "Email address is required"), Display(Name = "Email Address")]
        [EmailAddress]
        public string ShippingEmail { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        [Display(Name = "Organization or Company")]
        public string ShippingOrganization { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the confirm sale page id.
        /// </summary>
        public int ConfirmSalePageId { get; set; }
    }
}