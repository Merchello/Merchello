using umbraco.presentation.umbraco.dialogs;

namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core.Models;

    /// <summary>
    /// The customer display class.
    /// </summary>
    public class CustomerDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the login name.
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tax exempt.
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the collection of customer addresses.
        /// </summary>
        public IEnumerable<CustomerAddressDisplay> Addresses { get; set; }
    }

    #region Mapping Utility Extensions

    /// <summary>
    /// Internal mapping extensions for <see cref="CustomerDisplay"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class CustomerDisplayExtentions
    {
        /// <summary>
        /// Maps a <see cref="CustomerDisplay"/> to <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        internal static ICustomer ToCustomer(this CustomerDisplay customer, ICustomer destination)
        {
            if (!customer.Key.Equals(Guid.Empty)) destination.Key = customer.Key;

            destination.FirstName = customer.FirstName;
            destination.LastName = customer.LastName;

            // prevent setting email to empty string
            if (!string.IsNullOrEmpty(customer.Email))
            {
                destination.Email = customer.Email;
            }

            // prevent setting login name to empty string
            if (!string.IsNullOrEmpty(customer.LoginName))
            {
                ((Customer)destination).LoginName = customer.LoginName;
            }
            
            destination.TaxExempt = customer.TaxExempt;
            destination.LastActivityDate = DateTime.Now;

            if (customer.ExtendedData != null)
            {
                ((Customer)destination).ExtendedData = customer.ExtendedData;
            }

            var addressList = new List<ICustomerAddress>();

            foreach (var address in customer.Addresses)
            {
                var destAddress = destination.Addresses.FirstOrDefault(x => x.Key == address.Key);
                if (destAddress != null) destAddress = address.ToCustomerAddress(destAddress);

                addressList.Add(destAddress);
            }

            ((Customer) destination).Addresses = addressList;
            
            destination.Notes = customer.Notes;

            return destination;
        }

        /// <summary>
        /// Maps a <see cref="ICustomer"/> to <see cref="CustomerDisplay"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        internal static CustomerDisplay ToCustomerDisplay(this ICustomer customer)
        {
            var display = AutoMapper.Mapper.Map<CustomerDisplay>(customer);

            return display;
        }
    }

#endregion
}