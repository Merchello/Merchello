namespace Merchello.Web.Store.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Localization;

    using Umbraco.Core;

    /// <summary>
    /// A model used for collecting and persisting checkout addresses.
    /// </summary>
    public class CheckoutAddressModel : ICheckoutAddressModel //, IValidatableObject
    {
        /// <summary>
        /// Gets or sets the customer address key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets a label for a customer address.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelAddressLabel")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelOrganization")]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the name (full name) of the contact.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelName")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredName")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelAddress1")]
        [Required(ErrorMessageResourceName = "RequiredAddress", ErrorMessageResourceType = typeof(StoreFormsResource))]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelAddress2")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelLocality")]
        [Required(ErrorMessageResourceName = "RequiredLocality", ErrorMessageResourceType = typeof(StoreFormsResource))]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region or province.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelRegion")]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelPostalCode")]
        [Required(ErrorMessageResourceName = "RequiredPostalCode", ErrorMessageResourceType = typeof(StoreFormsResource))]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelCountry")]
        [Required(ErrorMessageResourceType = typeof(StoreFormsResource), ErrorMessageResourceName = "RequiredCountry")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the contact phone.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelPhone")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Display(ResourceType = typeof(StoreFormsResource), Name = "LabelEmailAddress")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        public AddressType AddressType { get; set; }

        /// <summary>
        /// Gets or sets the workflow marker.
        /// </summary>
        /// <remarks>
        /// This is used to assist in tracking the checkout - generally in single page checkouts
        /// </remarks>
        public ICheckoutWorkflowMarker WorkflowMarker { get; set; }

        ///// <summary>
        ///// Gets or sets a value indicating whether has provinces.
        ///// </summary>
        //internal bool HasProvinces { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (HasProvinces && Region.IsNullOrWhiteSpace()) yield return new ValidationResult(StoreFormsResource.RequiredRegion);
        //}
    }
}