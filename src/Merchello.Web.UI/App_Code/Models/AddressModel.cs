using Merchello.Core;
using Merchello.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;


namespace Models
{
    /// <summary>
    /// Summary description for AddressModel
    /// </summary>
    public class AddressModel
    {

        public Guid CustomerKey { get; set; }

        [Display(Name = "Address Line 1")]
        [Required(ErrorMessage = "Address line 1 is required.")]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [Display(Name = "Country")]
        public string CountryCode { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "This address is a commercial address (used in some shipping computations)")]
        public bool IsCommercial { get; set; }

        [Display(Name = "Locality or City")]
        [Required(ErrorMessage = "Country is required.")]
        public string Locality { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Display(Name = "Organization or Company Name")]
        public string Organization { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Postal or Zip Code")]
        [Required(ErrorMessage = "Postal code is required.")]
        public string PostalCode { get; set; }

        [Display(Name = "State or Region")]
        public string Region { get; set; }

        [Required]
        public AddressType AddressType { get; set; }
    }

    public static class AddressModelExtensions
    {
        public static IAddress ToAddress(this AddressModel address)
        {
            return new Address()
            {
                Address1 = address.Address1,
                Address2 = address.Address2,
                CountryCode = address.CountryCode,
                Email = address.Email,
                IsCommercial = address.IsCommercial,
                Locality = address.Locality,
                Name = address.Name,
                Organization = address.Organization,
                Phone = address.Phone,
                PostalCode = address.PostalCode,
                Region = address.Region
            };
        }
    }
}