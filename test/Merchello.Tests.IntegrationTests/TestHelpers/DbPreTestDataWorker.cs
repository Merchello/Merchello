using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.SqlSyntax;
using Moq;
using Umbraco.Core.Persistence;
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
        public UmbracoDatabase Database { get; private set; }

        public DbPreTestDataWorker()
            : this(new ServiceContext(new PetaPocoUnitOfWorkProvider()))
        { }

        internal DbSyntax SqlSyntax { get; set; }

        public DbPreTestDataWorker(ServiceContext serviceContext)
        {
            // sets up the Umbraco SqlSyntaxProvider Singleton
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            var uowProvider = new PetaPocoUnitOfWorkProvider();

            Database = uowProvider.GetUnitOfWork().Database;

            _serviceContext = serviceContext;
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
        /// Inserts a collection of address records into the database and returns a collection of <see cref="IAddress"/> objects representation
        /// </summary>        
        public IEnumerable<IAddress> MakeExistingAddressCollection(ICustomer customer, string label, int count)
        {
            var addresses = MockAddressDataMaker.AddressCollectionForInserting(customer, label, count);
            AddressService.Save(addresses);
            return addresses;
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

        #region IAnonymousCustomer


        public IAnonymousCustomer MakeExistingAnonymousCustomer()
        {
            var anonymous = MockAnonymousCustomerDataMaker.AnonymousCustomerForInserting();
            AnonymousCustomerService.Save(anonymous);
            return anonymous;
        }


        public IAnonymousCustomerService AnonymousCustomerService
        {
            get { return _serviceContext.AnonymousCustomerService; }
        }

        #endregion

        #region IBasket

        /// <summary>
        ///  Inserts an address record in the merchBasket table and returns an <see cref="IBasket"/> object representation
        /// </summary>
        public IBasket MakeExistingBasket(IConsumer consumer, BasketType basketType)
        {
            var basket = MockBasketDataMaker.ConsumerBasketForInserting(consumer, basketType);
            BasketService.Save(basket);
            return basket;
        }

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
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="maxItemCount">If itemCount is greater than 0, invoice items will be added to the invoice</param>
        /// <param name="customer"></param>
        /// <param name="invoiceStatus"></param>
        public IInvoice MakeExistingInvoice(ICustomer customer, IInvoiceStatus invoiceStatus, IAddress address, int maxItemCount = 0)
        {
            var invoice = MockInvoiceDataMaker.InvoiceForInserting(customer, invoiceStatus, address);
            InvoiceService.Save(invoice);

            if(maxItemCount > 0) MakeExistingInvoiceItemCollection(invoice, InvoiceItemType.Product, MockDataMakerBase.NoWhammyStop.Next(maxItemCount));

            return invoice;
        }

        /// <summary>
        /// Makes a list of invoices (without items) in the database and returns a collection of IInvoice representing these records
        /// </summary>
        public IEnumerable<IInvoice> MakeExistingInvoiceCollection(ICustomer customer, IInvoiceStatus invoiceStatus, IAddress address, int count)
        {
            var invoices = MockInvoiceDataMaker.InvoiceCollectionForInserting(customer, invoiceStatus, address, count);
            InvoiceService.Save(invoices);
            return invoices;
        }

        /// <summary>
        /// Makes a list of invoices (with items) in the database and returns a collection of IInvoice representing these records
        /// </summary>
        /// <param name="customer"><see cref="ICustomer"/></param>
        /// <param name="invoiceStatus"><see cref="IInvoiceStatus"/></param>
        /// <param name="address"><see cref="IAddress"/></param>
        /// <param name="count">the number of invoices to generate</param>
        /// <param name="maxItemCount">The maximum number of invoice items for each invoice</param>
        /// <returns></returns>
        public IEnumerable<IInvoice> MakeExistingInvoiceCollection(ICustomer customer, IInvoiceStatus invoiceStatus, IAddress address, int count, int maxItemCount)
        {
            for(var i = 0; i < count; i++) yield return MakeExistingInvoice(customer, invoiceStatus, address, maxItemCount);
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
        /// Makes an invoice item record in the database and returns an instance of the IInvoiceItem representing that record
        /// </summary>        
        public IInvoiceItem MakeExistingInvoiceItem(IInvoice invoice, InvoiceItemType invoiceItemType)
        {
            var invoiceItem = MockInvoiceItemDataMaker.InvoiceItemForInserting(invoice, invoiceItemType);
            InvoiceItemService.Save(invoiceItem);
            return invoiceItem;
        }

        /// <summary>
        /// Makes collection of invoice items associated with an invoice
        /// </summary>        
        public IEnumerable<IInvoiceItem> MakeExistingInvoiceItemCollection(IInvoice invoice, InvoiceItemType invoiceItemType, int count)
        {            
            var invoiceItems = MockInvoiceItemDataMaker.InvoiceItemCollectionForInserting(invoice, invoiceItemType, count);
            InvoiceItemService.Save(invoiceItems);
            return invoiceItems;
        }

        /// <summary>
        /// Deletes all invoice items
        /// </summary>
        public void DeleteAllInvoiceItems()
        {
            var all = ((InvoiceItemService) InvoiceItemService).GetAll();
            InvoiceItemService.Delete(all);
        }

        /// <summary>
        /// The invoice item service
        /// </summary>
        public IInvoiceItemService InvoiceItemService
        {
            get { return _serviceContext.InvoiceItemService; }
        }

        #endregion

        #region IProduct

        /// <summary>
        /// Saves a product record to the database and returns and instance of <see cref="IProductActual"/> represents that record
        /// </summary>
        /// <returns><see cref="IProductActual"/></returns>
        public IProductActual MakeExistingProduct()
        {
            var product = MockProductDataMaker.MockProductForInserting();
            ProductService.Save(product);
            return product;
        }

        /// <summary>
        /// Saves a collection of products to the database and return a collection of <see cref="IProductActual"/> representing that collection
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<IProductActual> MakeExistingProductCollection(int count)
        {
            var products = MockProductDataMaker.MockProductCollectionForInserting(count);
            ProductService.Save(products);
            return products;
        }

        /// <summary>
        /// Deletes all products
        /// </summary>
        public void DeleteAllProducts()
        {
            var all = ((ProductService) ProductService).GetAll();
            ProductService.Delete(all);
        }

        /// <summary>
        /// Gets the <see cref="IProductService"/>
        /// </summary>
        public IProductService ProductService
        {
            get { return _serviceContext.ProductService; }
        }

        #endregion

        #region IPayment



        /// <summary>
        /// Returns the Payment Service
        /// </summary>
        public IPaymentService PaymentService
        {
            get { return _serviceContext.PaymentService; }
        }

        #endregion

        #region IShipment


        public IShipmentService ShipmentService
        {
            get { return _serviceContext.ShipmentService; }
        }

        #endregion

        #region IShipMethod

        /// <summary>
        /// Gets the <see cref="IShipMethodService"/>
        /// </summary>
        public IShipMethodService ShipMethodService
        {
            get { return _serviceContext.ShipMethodService; }
        }

        

        #endregion

        #region IAppliedPayment


        /// <summary>
        /// Gets the transaction service
        /// </summary>
        public IAppliedPaymentService AppliedPaymentService
        {
            get { return _serviceContext.AppliedPaymentService; }
        }

        #endregion

        #region IWarehouse

        /// <summary>
        /// Gets <see cref="IWarehouseService"/>
        /// </summary>
        public IWarehouseService WarehouseService
        {
            get { return _serviceContext.WarehouseService; } 

        }
        #endregion

    }
}
