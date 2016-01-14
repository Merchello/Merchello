namespace Merchello.Bazaar.Models.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The customer address model.
    /// </summary>
    public partial class CustomerAddressModel
    {
        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Gets or sets the account page id.
        /// </summary>
        public int AccountPageId { get; set; }

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
        [Required(ErrorMessage = "Address 1 is required"), Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required(ErrorMessage = "Locality is required"), Display(Name = "Locality or city")]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [Display(Name = "Region, State or Province")]
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
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// The customer address model extentions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CustomerAddressModelExtentions
    {
        /// <summary>
        /// The as customer address.
        /// </summary>
        /// <param name="adr">
        /// The adr.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public static ICustomerAddress AsCustomerAddress(this CustomerAddressModel adr)
        {
            var customerAddress = new CustomerAddress(adr.CustomerKey)
                       {
                           Label = adr.Label,
                           FullName = adr.FullName,
                           Company = adr.Company,
                           AddressType = adr.AddressType == "shipping" ? AddressType.Shipping : AddressType.Billing,
                           Address1 = adr.Address1,
                           Address2 = adr.Address2,
                           Locality = adr.Locality,
                           Region = adr.Region,
                           PostalCode = adr.PostalCode,
                           CountryCode = adr.CountryCode,
                           Phone = adr.Phone,
                           IsDefault = false
                       };
            if (!adr.Key.Equals(Guid.Empty)) customerAddress.Key = adr.Key;
            return customerAddress;
        }

        /// <summary>
        /// The as customer address.
        /// </summary>
        /// <param name="adr">
        /// The adr.
        /// </param>
        /// <param name="existing">
        /// The existing.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public static ICustomerAddress AsCustomerAddress(this CustomerAddressModel adr, ICustomerAddress existing)
        {
            existing.Label = adr.Label;
            existing.FullName = adr.FullName;
            existing.Company = adr.Company;
            existing.AddressType = adr.AddressType == "shipping" ? AddressType.Shipping : AddressType.Billing;
            existing.Address1 = adr.Address1;
            existing.Address2 = adr.Address2;
            existing.Locality = adr.Locality;
            existing.Region = adr.Region;
            existing.PostalCode = adr.PostalCode;
            existing.CountryCode = adr.CountryCode;
            existing.Phone = adr.Phone;
            return existing;
        }
    }
}