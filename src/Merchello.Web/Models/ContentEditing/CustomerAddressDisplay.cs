﻿namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The customer address display.
    /// </summary>
    public class CustomerAddressDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the address label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the address type field key.
        /// </summary>
        public Guid AddressTypeFieldKey { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the address type.
        /// </summary>
        public AddressType AddressType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        public bool IsDefault { get; set; }
    }

    #region Mapping Extensions

    /// <summary>
    /// The customer address display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CustomerAddressDisplayExtensions
    {
        /// <summary>
        /// The to customer address display.
        /// </summary>
        /// <param name="customerAddress">
        /// The customer address.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerAddressDisplay"/>.
        /// </returns>
        public static CustomerAddressDisplay ToCustomerAddressDisplay(this ICustomerAddress customerAddress)
        {
            return AutoMapper.Mapper.Map<CustomerAddressDisplay>(customerAddress);
        }

        /// <summary>
        /// The to customer address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public static ICustomerAddress ToCustomerAddress(this CustomerAddressDisplay address)
        {
            return new CustomerAddress(address.CustomerKey)
                       {
                           Key = address.Key,
                           FullName = address.FullName,
                           Label = address.Label,
                           Address1 = address.Address1,
                           Address2 = address.Address2,
                           AddressTypeFieldKey = address.AddressTypeFieldKey,
                           Locality = address.Locality,
                           Region = address.Region,
                           PostalCode = address.PostalCode,
                           CountryCode = address.CountryCode,
                           Company = address.Company,
                           Phone = address.Phone,
                           IsDefault = address.IsDefault
                       };
        }
    }

    #endregion
}