namespace Merchello.Bazaar.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Umbraco.Core.Models;

    /// <summary>
    /// The checkout address form.
    /// </summary>
    public partial class CheckoutAddressForm
    {
        /// <summary>
        /// Gets or sets the theme name.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether save customer address.
        /// </summary>
        public bool SaveCustomerAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is anonymous.
        /// </summary>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Gets or sets the sale summary.
        /// </summary>
        public SalePreparationSummary SaleSummary { get; set; }

        #region Billing address

        /// <summary>
        /// Gets or sets the billing address key.
        /// </summary>
        public Guid BillingAddressKey { get; set; }

        /// <summary>
        /// Gets or sets the billing address label.
        /// </summary>
        [Display(Name = "Billing Address Label")]
        public string BillingAddressLabel { get; set; }

        /// <summary>
        /// Gets or sets the billing addresses.
        /// </summary>
        [Display(Name = "Select Saved Billing Address")]
        public IEnumerable<SelectListItem> BillingAddresses { get; set; }

            /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "Name is required"), Display(Name = "Name")]
        public string BillingName { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [Required(ErrorMessage = "Address 1 is required"), Display(Name = "Address 1")]
        public string BillingAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required(ErrorMessage = "Locality is required"), Display(Name = "Locality")]
        public string BillingLocality { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [Display(Name = "Address 2")]
        public string BillingAddress2 { get; set; }

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
        /// Gets or sets the shipping address key.
        /// </summary>
        public Guid ShippingAddressKey { get; set; }

        /// <summary>
        /// Gets or sets the shipping address label.
        /// </summary>
        [Display(Name = "Shipping Address Label")]
        public string ShippingAddressLabel { get; set; }

        /// <summary>
        /// Gets or sets the shipping addresses.
        /// </summary>
        [Display(Name = "Select Saved Shipping Address")]
        public IEnumerable<SelectListItem> ShippingAddresses { get; set; }

            /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "Name is required"), Display(Name = "Name")]
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
        [Display(Name = "Email Address")]
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

    /// <summary>
    /// The checkout address form extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CheckoutAddressFormExtensions
    {
        /// <summary>
        /// Gets the Billing Address from the model as an <see cref="IAddress"/>.
        /// </summary>
        /// <param name="form">
        /// The form.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress GetBillingAddress(this CheckoutAddressForm form)
        {
            return new Address()
                       {
                           AddressType = AddressType.Billing,
                           Name = form.BillingName,
                           Address1 = form.BillingAddress1,
                           Address2 = form.BillingAddress2,
                           Locality = form.BillingLocality,
                           Region = form.BillingRegion,
                           CountryCode = form.BillingCountryCode,
                           PostalCode = form.BillingPostalCode,
                           Email = form.BillingEmail,
                           Phone = form.BillingPhone
                       };
        }

        /// <summary>
        /// Gets the Billing Address from the model as an <see cref="IAddress"/>.
        /// </summary>
        /// <param name="form">
        /// The form.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress GetShippingAddress(this CheckoutAddressForm form)
        {
            return new Address()
                       {
                           AddressType = AddressType.Shipping,
                           Name = form.ShippingName,
                           Address1 = form.ShippingAddress1,
                           Address2 = form.ShippingAddress2,
                           Locality = form.ShippingLocality,
                           Region = form.ShippingRegion,
                           CountryCode = form.ShippingCountryCode,
                           PostalCode = form.ShippingPostalCode,
                           Email = form.ShippingEmail,
                           Phone = form.ShippingPhone
                       };
        }
    }
}
