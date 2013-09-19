using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Tests.IntegrationTests.TestHelpers
{
    /// <summary>
    /// Assists with integration tests which require data to be present in the database and is useful in
    /// quickly populating the database with data for UI testing.
    /// </summary>
    public class DbPreTestDataWorker
    {
        
        private readonly ServiceContext _serviceContext;

        public DbPreTestDataWorker()
        {
            // sets up the Umbraco SqlSyntaxProvider Singleton
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            _serviceContext = new ServiceContext(new PetaPocoUnitOfWorkProvider());
        }


        #region IAddress

        /// <summary>
        /// Inserts an address record in the merchAddress table and returns an <see cref="IAddress"/> object representation
        /// </summary>
        /// <returns></returns>
        public IAddress MakeExistingAddress()
        {
            return MakeExistingAddress(MakeExistingCustomer(), "Home");
        }

        /// <summary>
        /// Inserts an address record in the merchAddress table and returns an <see cref="IAddress"/> object representation
        /// </summary>
        public IAddress MakeExistingAddress(ICustomer customer, string label)
        {
            var address = MockAddressDataMaker.RandomAddress(customer, label);
            AddressService.Save(address);
            return address;
        }

        /// <summary>
        /// Deletes all of the addresses in the database
        /// </summary>
        public void DeleteAllAddresses()
        {
            var all = ((AddressService) AddressService).GetAll();
            AddressService.Delete(all);
        }

        /// <summary>
        /// The address service
        /// </summary>
        public IAddressService AddressService
        {
            get { return _serviceContext.AddressService; }
        }

        #endregion

        #region ICustomer

        /// <summary>
        /// Inserts a customer record in the merchCustomer table and returns an <see cref="ICustomer"/> object representation
        /// </summary>
        /// <returns></returns>
        public ICustomer MakeExistingCustomer()
        {
            var customer = MockCustomerDataMaker.CustomerForInserting();
            CustomerService.Save(customer);
            return customer;
        }

        /// <summary>
        /// Inserts a collection customer records in the merchCustomer table and returns a collection <see cref="ICustomer"/> object representation
        /// </summary>
        /// <param name="count">The number of customers to create in the collection</param>
        /// <returns></returns>
        public IEnumerable<ICustomer> CollectionExistingCustomers(int count)
        {
            var customers = new List<ICustomer>();
            for(var i =0; i < count; i++) customers.Add(MockCustomerDataMaker.CustomerForInserting());
            CustomerService.Save(customers);
            return customers;
        }

        /// <summary>
        /// Deletes all of the customers from the database
        /// </summary>
        public void DeleteAllCustomers()
        {
            var all = ((CustomerService) CustomerService).GetAll();
            CustomerService.Delete(all);
        }

        /// <summary>
        /// The customer service
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _serviceContext.CustomerService; }
        }

        #endregion

        #region IInvoiceStatus

        public IEnumerable<IInvoiceStatus> DefaultInvoiceStatuses()
        {            
            var statuses = InvoiceStatusService.GetAll();
            if (!statuses.Any())
            {
                // populate the table
                var creation = new BaseDataCreation(new PetaPocoUnitOfWorkProvider().GetUnitOfWork().Database);
                creation.InitializeBaseData("merchInvoiceStatus");               
            }
            return InvoiceStatusService.GetAll();
        }

        public IInvoiceStatusService InvoiceStatusService
        {
            get { return _serviceContext.InvoiceStatusService; }
        }
        #endregion

        #region IInvoice

        /// <summary>
        /// Makes an invoice record in the database and returns an instance of IInvoice representing that record
        /// </summary>
        public IInvoice MakeExistingInvoice(ICustomer customer, IInvoiceStatus invoiceStatus, IAddress address)
        {
            var invoice = InvoiceService.CreateInvoice(customer, address, invoiceStatus, Guid.NewGuid().ToString().Substring(0, 8));
            InvoiceService.Save(invoice);
            return invoice;
        }

        /// <summary>
        /// Deletes all invoices
        /// </summary>
        public void DeleteAllInvoices()
        {
            var all = ((InvoiceService)InvoiceService).GetAll().ToArray();
            InvoiceService.Delete(all);
        }

        /// <summary>
        /// The invoice service
        /// </summary>
        public IInvoiceService InvoiceService
        {
            get { return _serviceContext.InvoiceService; }
        }

        #endregion

        #region IInvoiceItem

        /// <summary>
        /// The invoice item service
        /// </summary>
        public IInvoiceItemService InvoiceItemService
        {
            get { return _serviceContext.InvoiceItemService; }
        }

        #endregion


        #region IBasket

        /// <summary>
        /// The basket service
        /// </summary>
        public IBasketService BasketService
        {
            get { return _serviceContext.BasketService; }
        }

        #endregion


        #region IBasketItem

        /// <summary>
        /// The basket item service
        /// </summary>
        public IBasketItemService BasketItemService
        {
            get { return _serviceContext.BasketItemService; }
        }

        #endregion


        #region IShipment


        public IShipmentService ShipmentService
        {
            get { return _serviceContext.ShipmentService; }
        }

        #endregion



        #region IShipMethod

        public IShipMethodService ShipMethodService
        {
            get { return _serviceContext.ShipMethodService; }
        }

        #endregion

    }
}
