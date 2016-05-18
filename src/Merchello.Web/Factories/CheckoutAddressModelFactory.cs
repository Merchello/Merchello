namespace Merchello.Web.Factories
{
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A factory to create <see cref="ICheckoutAddressModel"/>.
    /// </summary>
    /// <typeparam name="TAddress">
    /// The type of checkout address
    /// </typeparam>
    /// <remarks>
    /// This allows for custom (site specific) addresses to be used in the checkout by
    /// overriding the "OnCreate" virtual method.
    /// </remarks>
    public class CheckoutAddressModelFactory<TAddress>
        where TAddress : class, ICheckoutAddressModel, new()
    {

        /// <summary>
        /// Creates a <see cref="IAddress"/> from <see cref="ICheckoutAddressModel"/>.
        /// </summary>
        /// <param name="adr">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public IAddress Create(TAddress adr)
        {
            return new Address
                {
                    Name = adr.Name,
                    Organization = adr.Organization,
                    Email = adr.Email,
                    Address1 = adr.Address1,
                    Address2 = adr.Address2,
                    Locality = adr.Locality,
                    Region = adr.Region,
                    CountryCode = adr.CountryCode,
                    PostalCode = adr.PostalCode,
                    Phone = adr.Phone,
                    AddressType = adr.AddressType,
                };
        }

        /// <summary>
        /// Creates a <see cref="ICheckoutAddressModel"/> from a <see cref="IAddress"/>.
        /// </summary>
        /// <param name="adr">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        public TAddress Create(IAddress adr)
        {   
            var address = new TAddress
                {
                    Name = adr.Name,
                    Organization = adr.Organization,
                    Email = adr.Email,
                    Address1 = adr.Address1,
                    Address2 = adr.Address2,
                    Locality = adr.Locality,
                    Region = adr.Region,
                    CountryCode = adr.CountryCode,
                    PostalCode = adr.PostalCode,
                    Phone = adr.Phone,
                    AddressType = adr.AddressType,
                };

            return OnCreate(address, adr);
        }

        /// <summary>
        /// Creates a <see cref="ICheckoutAddressModel"/> from a <see cref="ICustomerAddress"/>.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="adr">
        /// The <see cref="ICustomerAddress"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        public TAddress Create(ICustomer customer, ICustomerAddress adr)
        {
            return new TAddress
            {
                Name = adr.FullName,
                Organization = adr.Company,
                Email = customer.Email,
                Address1 = adr.Address1,
                Address2 = adr.Address2,
                Locality = adr.Locality,
                Region = adr.Region,
                CountryCode = adr.CountryCode,
                PostalCode = adr.PostalCode,
                Phone = adr.Phone,
                AddressType = adr.AddressType,
            };
        }

        /// <summary>
        ///  Allows for overriding the creation of <see cref="ICheckoutAddressModel"/> from <see cref="IAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        protected virtual TAddress OnCreate(TAddress address, IAddress adr)
        {
            return address;
        }
    }
}