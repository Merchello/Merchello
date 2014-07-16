﻿namespace Merchello.Core.Models
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Services;
    using Newtonsoft.Json;

    /// <summary>
    /// The customer extensions.
    /// </summary>
    public static class CustomerExtensions
    {
        /// <summary>
        /// Gets a collection of all customer addresses
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The collection of all <see cref="ICustomerAddress"/> for a given customer
        /// </returns>
        public static IEnumerable<ICustomerAddress> Addresses(this ICustomer customer)
        {
            return customer.Addresses(MerchelloContext.Current);
        }

        /// <summary>
        /// The addresses.
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
        public static IEnumerable<ICustomerAddress> Addresses(this ICustomer customer, AddressType addressType)
        {
            return customer.Addresses(MerchelloContext.Current, addressType);
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
        /// <param name="addressType">The <see cref="AddressType"/></param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        public static ICustomerAddress CreateCustomerAddress(this ICustomer customer, IAddress address, AddressType addressType)
        {
            return customer.CreateCustomerAddress(MerchelloContext.Current, address, addressType);
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
        internal static IEnumerable<ICustomerAddress> Addresses(this ICustomer customer, IMerchelloContext merchelloContext)
        {
            return ((ServiceContext) merchelloContext.Services).CustomerAddressService.GetByCustomerKey(customer.Key);
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
        internal static IEnumerable<ICustomerAddress> Addresses(this ICustomer customer, IMerchelloContext merchelloContext, AddressType addressType)
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
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        internal static ICustomerAddress CreateCustomerAddress(this ICustomer customer, IMerchelloContext merchelloContext, IAddress address, AddressType addressType)
        {
            var customerAddress = address.ToCustomerAddress(customer, addressType);

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

        #endregion
    }
}