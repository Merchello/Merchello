namespace Merchello.QuickMart.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Merchello.Core;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A model used for collecting and persisting checkout addresses.
    /// </summary>
    public class CheckoutAddressModel : ICheckoutAddressModel
    {
        /// <summary>
        /// Gets or sets the customer address key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets a label for a customer address.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the name (full name) of the contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        [Required]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Required]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region or province.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Required]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [Required]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the contact phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        public IEnumerable<CountryDisplay> Countries { get; set; }

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
    }
}