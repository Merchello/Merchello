namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Defines an address used in checkout implementations.
    /// </summary>
    /// <remarks>
    /// This interface asserts the address collected can be used in invoices, shipments and be persisted
    /// as a <see cref="ICustomerAddress"/>
    /// </remarks>
    public interface ICheckoutAddressModel : ICheckoutModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <remarks>
        /// This the Merchello customer address key
        /// </remarks>
        Guid Key { get; set; }

        /// <summary>
        /// Gets or sets a label for the address (e.g. My House).
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        string Organization { get; set; }

        /// <summary>
        /// Gets or sets the name (full name) of the contact.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region or province.
        /// </summary>
        string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the contact phone.
        /// </summary>
        string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        string Email { get; set; }

            /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        AddressType AddressType { get; set; }
    }
}