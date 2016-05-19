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
        /// Creates <see cref="ICustomerAddress"/> from <see cref="ICheckoutAddressModel"/>.
        /// </summary>
        /// <param name="adr">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomer"/>.
        /// </param>
        /// <param name="label">
        /// The customer address label (e.g. My House).
        /// </param>
        /// <param name="addressType">
        /// The <see cref="AddressType"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public ICustomerAddress Create(TAddress adr, ICustomer customer, string label, AddressType addressType)
        {
            var model = Create(adr).ToCustomerAddress(customer, label, addressType);

            return OnCreate(model, adr, customer, label, addressType);
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
            var model = new TAddress
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

            return OnCreate(model, adr);
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
            var model = new TAddress
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

            return OnCreate(model, customer, adr);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutAddressModel"/> from <see cref="ICustomerAddress"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomer"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="ICustomerAddress"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        protected virtual TAddress OnCreate(TAddress model, ICustomer customer, ICustomerAddress adr)
        {
            return model;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICustomerAddress"/> from <see cref="ICheckoutAddressModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICustomerAddress"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomer"/>.
        /// </param>
        /// <param name="label">
        /// The customer address label (e.g. My House).
        /// </param>
        /// <param name="addressType">
        /// The <see cref="AddressType"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICustomerAddress"/>.
        /// </returns>
        protected virtual ICustomerAddress OnCreate(ICustomerAddress model, TAddress adr, ICustomer customer, string label, AddressType addressType)
        {
            return model;
        }

        /// <summary>
        ///  Allows for overriding the creation of <see cref="ICheckoutAddressModel"/> from <see cref="IAddress"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="adr">
        /// The <see cref="IAddress"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutAddressModel"/>.
        /// </returns>
        protected virtual TAddress OnCreate(TAddress model, IAddress adr)
        {
            return model;
        }
    }
}