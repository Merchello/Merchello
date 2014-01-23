using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.SqlSyntax;
using Moq;
using Umbraco.Core.Persistence;


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
        public IWarehouseCatalog _warehouseCatalog;
        public DbPreTestDataWorker()
            : this(new ServiceContext(new PetaPocoUnitOfWorkProvider()))
        { }

        internal DbSyntax SqlSyntax { get; set; }

        public DbPreTestDataWorker(ServiceContext serviceContext)
        {
            var syntax = (DbSyntax)Enum.Parse(typeof (DbSyntax), ConfigurationManager.AppSettings["syntax"]);
            // sets up the Umbraco SqlSyntaxProvider Singleton
            SqlSyntaxProviderTestHelper.EstablishSqlSyntax(syntax);

            var uowProvider = new PetaPocoUnitOfWorkProvider();

            Database = uowProvider.GetUnitOfWork().Database;

            _serviceContext = serviceContext;

            _warehouseCatalog = new WarehouseCatalog(Constants.DefaultKeys.DefaultWarehouseKey)
            {
                Key = Constants.DefaultKeys.DefaultWarehouseCatalogKey
            };
        }

        #region IAddress

        /// <summary>
        /// Inserts an address record in the merchAddress table and returns an <see cref="ICustomerAddress"/> object representation
        /// </summary>
        /// <returns></returns>
        public ICustomerAddress MakeExistingAddress()
        {
            return MakeExistingAddress(MakeExistingCustomer(), "Home");
        }

        /// <summary>
        /// Inserts an address record in the merchAddress table and returns an <see cref="ICustomerAddress"/> object representation
        /// </summary>
        public ICustomerAddress MakeExistingAddress(ICustomer customer, string label)
        {
            var address = MockCustomerAddressDataMaker.RandomAddress(customer, label);
            //AddressService.Save(address);
            return address;
        }

        /// <summary>
        /// Inserts a collection of address records into the database and returns a collection of <see cref="ICustomerAddress"/> objects representation
        /// </summary>        
        public IEnumerable<ICustomerAddress> MakeExistingAddressCollection(ICustomer customer, string label, int count)
        {
            var addresses = MockCustomerAddressDataMaker.AddressCollectionForInserting(customer, label, count);
            //AddressService.Save(addresses);
            return addresses;
        }

        /// <summary>
        /// Deletes all of the addresses in the database
        /// </summary>
        public void DeleteAllAddresses()
        {
            //var all = ((AddressService) AddressService).GetAll();
            //AddressService.Delete(all);
        }

        ///// <summary>
        ///// The address service
        ///// </summary>
        //public IAddressService AddressService
        //{
        //    get { return _serviceContext.AddressService; }
        //}

        #endregion

        #region IAnonymousCustomer


        public IAnonymousCustomer MakeExistingAnonymousCustomer()
        {
            var anonymous = MockAnonymousCustomerDataMaker.AnonymousCustomerForInserting();
            CustomerService.Save(anonymous);
            //anonymous.Key = Guid.NewGuid();
            //anonymous.ResetDirtyProperties();
            return anonymous;
        }

        public void DeleteAllAnonymousCustomers()
        {
            var allAnonymous = ((CustomerService) CustomerService).GetAllAnonymousCustomers();
            CustomerService.Delete(allAnonymous);
        }

        #endregion

        #region IItemCache

        /// <summary>
        ///  Inserts an address record in the merchBasket table and returns an <see cref="IItemCache"/> object representation
        /// </summary>
        public IItemCache MakeExistingItemCache(ICustomerBase customer, ItemCacheType itemCacheType)
        {
            var itemCache = MockCustomerItemCacheDataMaker.ConsumerItemCacheForInserting(customer, itemCacheType);
            ItemCacheService.Save(itemCache);
            return itemCache;
        }

        public void DeleteAllItemCaches()
        {
            var all = ((ItemCacheService) ItemCacheService).GetAll();
            ItemCacheService.Delete(all);
        }

        /// <summary>
        /// The customer item cache service
        /// </summary>
        public IItemCacheService ItemCacheService
        {
            get { return _serviceContext.ItemCacheService; }
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

        #region IGatewayProvider

        /// <summary>
        /// Gets the gateway provider service
        /// </summary>
        public IGatewayProviderService GatewayProviderService
        {
            get { return _serviceContext.GatewayProviderService; }
        }

        #endregion

        #region IInvoice

        ///// <summary>
        ///// Makes an invoice record in the database and returns an instance of IInvoice representing that record
        ///// 
        ///// </summary>
        ///// <param name="customerAddress"></param>
        ///// <param name="maxItemCount">If itemCount is greater than 0, invoice items will be added to the invoice</param>
        ///// <param name="customer"></param>
        ///// <param name="invoiceStatus"></param>
        //public IInvoice MakeExistingInvoice(ICustomer customer, IInvoiceStatus invoiceStatus, ICustomerAddress customerAddress, int maxItemCount = 0)
        //{
        //    var invoice = MockInvoiceDataMaker.InvoiceForInserting(customer, invoiceStatus, customerAddress);
        //    InvoiceService.Save(invoice);

        //    //if(maxItemCount > 0) MakeExistingInvoiceItemCollection(invoice, InvoiceItemType.Product, MockDataMakerBase.NoWhammyStop.Next(maxItemCount));

        //    return invoice;
        //}

        ///// <summary>
        ///// Makes a list of invoices (without items) in the database and returns a collection of IInvoice representing these records
        ///// </summary>
        //public IEnumerable<IInvoice> MakeExistingInvoiceCollection(ICustomer customer, IInvoiceStatus invoiceStatus, ICustomerAddress customerAddress, int count)
        //{
        //    var invoices = MockInvoiceDataMaker.InvoiceCollectionForInserting(customer, invoiceStatus, customerAddress, count);
        //    InvoiceService.Save(invoices);
        //    return invoices;
        //}

        ///// <summary>
        ///// Makes a list of invoices (with items) in the database and returns a collection of IInvoice representing these records
        ///// </summary>
        ///// <param name="customer"><see cref="ICustomer"/></param>
        ///// <param name="invoiceStatus"><see cref="IInvoiceStatus"/></param>
        ///// <param name="customerAddress"><see cref="ICustomerAddress"/></param>
        ///// <param name="count">the number of invoices to generate</param>
        ///// <param name="maxItemCount">The maximum number of invoice items for each invoice</param>
        ///// <returns></returns>
        //public IEnumerable<IInvoice> MakeExistingInvoiceCollection(ICustomer customer, IInvoiceStatus invoiceStatus, ICustomerAddress customerAddress, int count, int maxItemCount)
        //{
        //    for(var i = 0; i < count; i++) yield return MakeExistingInvoice(customer, invoiceStatus, customerAddress, maxItemCount);
        //}


        ///// <summary>
        ///// Deletes all invoices
        ///// </summary>
        //public void DeleteAllInvoices()
        //{
        //    var all = ((InvoiceService)InvoiceService).GetAll().ToArray();
        //    InvoiceService.Delete(all);
        //}

        ///// <summary>
        ///// The invoice service
        ///// </summary>
        //public IInvoiceService InvoiceService
        //{
        //    get { return _serviceContext.InvoiceService; }
        //}

        #endregion

        

        #region IProduct

        /// <summary>
        /// Saves a product record to the database and returns and instance of <see cref="IProduct"/> represents that record
        /// </summary>
        /// <returns><see cref="IProduct"/></returns>
        public IProduct MakeExistingProduct(bool shippable = true)
        {
            var product = MockProductDataMaker.MockProductForInserting(shippable);            
            ProductService.Save(product);
            product.AddToCatalogInventory(_warehouseCatalog);
            ProductService.Save(product);
            return product;
        }

        /// <summary>
        /// Saves a collection of products to the database and return a collection of <see cref="IProduct"/> representing that collection
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<IProduct> MakeExistingProductCollection(int count)
        {
            var products = MockProductDataMaker.MockProductCollectionForInserting(count);
            foreach (var p in products)
            {
                ProductService.Save(p);
                yield return p;
            }
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

        #region IProductVariant


        /// <summary>
        /// Gets the <see cref="IProductVariantService"/>
        /// </summary>
        public IProductVariantService ProductVariantService
        {
            get { return _serviceContext.ProductVariantService; }
        }

        #endregion

        #region IPayment



        ///// <summary>
        ///// Returns the Payment Service
        ///// </summary>
        //public IPaymentService PaymentService
        //{
        //    get { return _serviceContext.PaymentService; }
        //}

        #endregion


        #region Shipping (IShipment, IShipCounty)

        public void DeleteAllShipCountries()
        {
            var shipCountries = ((ShippingService) ShippingService).GetAllShipCountries();
            foreach (var country in shipCountries)
            {
                ShippingService.Delete(country);
            }

        }

        /// <summary>
        /// Returns the Shipping Service
        /// </summary>
        public IShippingService ShippingService
        {
            get
            {
                return _serviceContext.ShippingService;
            }
        }

        #endregion

        #region Settings

        /// <summary>
        /// Returns the <see cref="IStoreSettingService"/>
        /// </summary>
        public IStoreSettingService StoreSettingService
        {
            get { return _serviceContext.StoreSettingService; }
        }

        #endregion

        //#region IShipment


        //public IShippingService ShippingService
        //{
        //    get { return _serviceContext.ShippingService; }
        //}

        //#endregion


        #region IAppliedPayment



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
