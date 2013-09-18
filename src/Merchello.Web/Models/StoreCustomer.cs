using System;
using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Web.Models
{
    public class StoreCustomer : IStoreCustomer
    {
        private readonly ICustomer _customer;
        private Lazy<IEnumerable<IInvoice>> _invoices;
        private Lazy<IEnumerable<IPayment>> _payments;
        private Lazy<IEnumerable<IAddress>> _addresses;
        private readonly ICustomerService _customerService;
        private readonly IInvoiceService _invoiceService;
        private readonly IAddressService _addressService;

        public StoreCustomer(ICustomer customer)
            : this(customer, MerchelloContext.Current)
        { }

        public StoreCustomer(ICustomer customer, MerchelloContext merchelloContext)
            : this(customer, merchelloContext.Services.CustomerService, merchelloContext.Services.InvoiceService, merchelloContext.Services.AddressService)
        { }

        public StoreCustomer(ICustomer customer, ICustomerService customerService, IInvoiceService invoiceService, IAddressService addressService)
        {
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(customerService, "customerService");
            Mandate.ParameterNotNull(invoiceService, "invoiceService");
            Mandate.ParameterNotNull(addressService, "addressService");

            _customer = customer;
            _customerService = customerService;
            _invoiceService = invoiceService;
            _addressService = addressService;
         
        }

        /// <summary>
        /// The customer information
        /// </summary>
        public ICustomer Customer { get { return _customer;  } }

        /// <summary>
        /// A collection of <see cref="IInvoice"/> charged to this customer
        /// </summary>
        public IEnumerable<IInvoice> Invoices
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// A collection of <see cref="IPayment"/> for the customer
        /// </summary>
        public IEnumerable<IPayment> Payments
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// A collection of <see cref="IAddress"/> associated with the customer
        /// </summary>
        public IEnumerable<IAddress> Addresses
        {
            get { throw new NotImplementedException(); }
        }
    }


}
