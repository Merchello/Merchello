namespace Merchello.Plugin.Payments.Braintree
{
    using System.Linq;

    using global::Braintree;

    using Merchello.Core.Models;

    using Address = global::Braintree.Address;

    /// <summary>
    /// Utility extensions for the Braintree <see cref="Customer"/>.
    /// </summary>
    public static class BraintreeCustomerExtensions
    {
        /// <summary>
        /// Finds an existing Braintree customer <see cref="Address"/> that matches an <see cref="IAddress"/>.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="Address"/>.
        /// </returns>
        public static Address GetMatchingAddress(this Customer customer, IAddress address)
        {
            return !customer.Addresses.Any() ? null : 
                customer.Addresses.FirstOrDefault(x => x.IsEqual(address));
        }

        /// <summary>
        /// Performs an equality comparison between a Braintree <see cref="Address"/> and a Merchello <see cref="IAddress"/>.
        /// </summary>
        /// <param name="bta">
        /// The Braintree address.
        /// </param>
        /// <param name="ma">
        /// The Merchello address.
        /// </param>
        /// <returns>
        /// Returns true if address properties match.
        /// </returns>
        private static bool IsEqual(this Address bta, IAddress ma)
        {
            var initial = bta.Company == ma.Organization && bta.StreetAddress == ma.Address1
                   && bta.ExtendedAddress == ma.Address2 && bta.Locality == ma.Locality && bta.Region == ma.Region
                   && bta.CountryCodeAlpha2 == ma.CountryCode && bta.PostalCode == ma.PostalCode;

            if (!initial) return false;


            if (!string.IsNullOrEmpty(ma.Name))
            {
                return bta.FirstName == ma.TrySplitFirstName() && bta.LastName == ma.TrySplitLastName();
            }

            return true;
        }
    }
}