namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using Newtonsoft.Json;

    using Services;
   
    /// <summary>
    /// The customer extensions.
    /// </summary>
    public static class CustomerExtensions
    {
        /// <summary>
        /// Maps a <see cref="ICustomerAddress"/> to a <see cref="IAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress AsAddress(this ICustomerAddress address, string name)
        {
            return new Address()
                       {
                           Name = name,
                           Organization = address.Company,
                           Address1 = address.Address1,
                           Address2 = address.Address2,
                           Locality = address.Locality,
                           Region = address.Region,
                           PostalCode = address.PostalCode,
                           CountryCode = address.CountryCode,
                           Phone = address.Phone,
                           AddressType = address.AddressType
                       };
        }

        /// <summary>
        /// The default customer address associated with a customer of a given type
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The collection of <see cref="ICustomerAddress"/>
        /// </returns>
        public static ICustomerAddress DefaultCustomerAddress(this ICustomer customer, AddressType addressType)
        {
            return customer.DefaultCustomerAddress(MerchelloContext.Current, addressType);
        }

        /// <summary>
        /// Creates a <see cref="ICustomerAddress"/> based off an <see cref="IAddress"/>
        /// </summary>
        /// <param name="customer">
        /// The customer associated with the address
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="label">
        /// The address label
        /// </param>
        /// <param name="addressType">
        /// The <see cref="AddressType"/>
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public static ICustomerAddress CreateCustomerAddress(this ICustomer customer, IAddress address, string label, AddressType addressType)
        {
            return customer.CreateCustomerAddress(MerchelloContext.Current, address, label, addressType);
        }

        /// <summary>
        /// The <see cref="ICustomerAddress"/> to be saved
        /// </summary>
        /// <param name="customer">
        /// The customer associated with the address
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public static ICustomerAddress SaveCustomerAddress(this ICustomer customer, ICustomerAddress address)
        {
            return customer.SaveCustomerAddress(MerchelloContext.Current, address);
        }

        /// <summary>
        /// Deletes a customer address.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="address">
        /// The address to be deleted
        /// </param>
        public static void DeleteCustomerAddress(this ICustomer customer, ICustomerAddress address)
        {
            customer.DeleteCustomerAddress(MerchelloContext.Current, address);
        }

        /// <summary>
        /// Gets a collection of <see cref="IInvoice"/> associated with the customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IInvoice"/>.
        /// </returns>
        public static IEnumerable<IInvoice> Invoices(this ICustomer customer)
        {
            return customer.Invoices(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> associated with the customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IPayment"/>
        /// </returns>
        public static IEnumerable<IPayment> Payments(this ICustomer customer)
        {
            return customer.Payments(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets a collection of addresses associated with the customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The collection of <see cref="ICustomerAddress"/> associated with the customer
        /// </returns>
        internal static IEnumerable<ICustomerAddress> CustomerAddresses(this ICustomer customer, IMerchelloContext merchelloContext)
        {
            return ((ServiceContext)merchelloContext.Services).CustomerAddressService.GetByCustomerKey(customer.Key);
        }

        /// <summary>
        /// The addresses.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="addressType">
        /// The address Type.
        /// </param>
        /// <returns>
        /// The collection of <see cref="ICustomerAddress"/> associated with the customer of a given type
        /// </returns>
        internal static IEnumerable<ICustomerAddress> CustomerAddresses(this ICustomer customer, IMerchelloContext merchelloContext, AddressType addressType)
        {
            return ((ServiceContext)merchelloContext.Services).CustomerAddressService.GetByCustomerKey(customer.Key, addressType);
        }

        /// <summary>
        /// The default customer address.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The default <see cref="ICustomerAddress"/> of a given type
        /// </returns>
        internal static ICustomerAddress DefaultCustomerAddress(this ICustomer customer, IMerchelloContext merchelloContext, AddressType addressType)
        {
            return ((ServiceContext)merchelloContext.Services).CustomerAddressService.GetDefaultCustomerAddress(customer.Key, addressType);
        }

        /// <summary>
        /// The create customer address.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="label">
        /// The customer label
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        internal static ICustomerAddress CreateCustomerAddress(this ICustomer customer, IMerchelloContext merchelloContext, IAddress address, string label, AddressType addressType)
        {
            var customerAddress = address.ToCustomerAddress(customer, label, addressType);

            return customer.SaveCustomerAddress(merchelloContext, customerAddress);
        }

        /// <summary>
        /// Saves customer address.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        internal static ICustomerAddress SaveCustomerAddress(this ICustomer customer, IMerchelloContext merchelloContext, ICustomerAddress address)
        {
            Mandate.ParameterCondition(address.CustomerKey == customer.Key, "The customer address is not associated with this customer.");

            ((ServiceContext)merchelloContext.Services).CustomerAddressService.Save(address);

            var addresses = customer.Addresses.ToList();

            if (addresses.Any(x => x.Key == address.Key))
            {
                addresses.RemoveAt(addresses.IndexOf(addresses.FirstOrDefault(x => x.Key == address.Key)));
            }

            addresses.Add(address);


            ((Customer)customer).Addresses = addresses;

            return address;
        }

        /// <summary>
        /// The delete customer address.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        internal static void DeleteCustomerAddress(this ICustomer customer, IMerchelloContext merchelloContext, ICustomerAddress address)
        {
            Mandate.ParameterCondition(address.CustomerKey == customer.Key, "The customer address is not associated with this customer.");

            ((ServiceContext)merchelloContext.Services).CustomerAddressService.Delete(address);

            var addresses = customer.Addresses.ToList();

            if (addresses.Any(x => x.Key == address.Key))
            {
                addresses.RemoveAt(addresses.IndexOf(addresses.FirstOrDefault(x => x.Key == address.Key)));
            }

            ((Customer)customer).Addresses = addresses;
        }

        /// <summary>
        /// Gets the collection of <see cref="IInvoice"/> associated with the customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IInvoice"/>.
        /// </returns>
        internal static IEnumerable<IInvoice> Invoices(this ICustomer customer, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Services.InvoiceService.GetInvoicesByCustomerKey(customer.Key);
        }

        /// <summary>
        /// Gets the collection of <see cref="IPayment"/> associated with a customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IPayment"/>.
        /// </returns>
        internal static IEnumerable<IPayment> Payments(this ICustomer customer, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Services.PaymentService.GetPaymentsByCustomerKey(customer.Key);
        }

        #region Examine Serialization

        /// <summary>
        /// Serializes a customer to xml suitable for Examine indexer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="XDocument"/>.
        /// </returns>
        internal static XDocument SerializeToXml(this ICustomer customer)
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("customer");
                    writer.WriteAttributeString("id", ((Customer)customer).ExamineId.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("customerKey", customer.Key.ToString());
                    writer.WriteAttributeString("loginName", customer.LoginName);
                    writer.WriteAttributeString("firstName", customer.FirstName);
                    writer.WriteAttributeString("lastName", customer.LastName);
                    writer.WriteAttributeString("email", customer.Email);
                    writer.WriteAttributeString("taxExempt", customer.TaxExempt.ToString());
                    writer.WriteAttributeString("extendedData", customer.ExtendedDataAsJson());
                    writer.WriteAttributeString("notes", customer.Notes);
                    writer.WriteAttributeString("addresses", customer.AddressesAsJson());
                    writer.WriteAttributeString("lastActivityDate", customer.LastActivityDate.ToString("s"));
                    writer.WriteAttributeString("createDate", customer.CreateDate.ToString("s"));
                    writer.WriteAttributeString("updateDate", customer.UpdateDate.ToString("s"));
                    writer.WriteAttributeString("allDocs", "1");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    xml = sw.ToString();
                }
            }

            return XDocument.Parse(xml);
        }

        /// <summary>
        /// The customer address collection as JSON.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The JSON representation <see cref="string"/>.
        /// </returns>
        private static string AddressesAsJson(this ICustomer customer)
        {
            return JsonConvert.SerializeObject(customer.Addresses ?? new List<ICustomerAddress>());
        }

        #endregion
    }
}