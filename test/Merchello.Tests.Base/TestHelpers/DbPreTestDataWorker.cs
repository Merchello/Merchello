using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Migrations.Initial;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.SqlSyntax;
using Umbraco.Core.Persistence;

namespace Merchello.Tests.Base.TestHelpers
{
    /// <summary>
    /// Assists with integration tests which require data to be present in the database and is useful in
    /// quickly populating the database with data for UI testing.
    /// </summary>
    public class DbPreTestDataWorker
    {
        
        private readonly ServiceContext _serviceContext;
        public UmbracoDatabase Database { get; private set; }
        public IWarehouseCatalog WarehouseCatalog;
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

            WarehouseCatalog = new WarehouseCatalog(Constants.DefaultKeys.Warehouse.DefaultWarehouseKey)
            {
                Key = Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey
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


        #region ICountryTaxRegion

        /// <summary>
        /// Deletes all country tax rates for a given provider
        /// </summary>
        /// <param name="providerKey"></param>
        public void DeleteAllCountryTaxRates(Guid providerKey)
        {
            var countryTaxRates = TaxMethodService.GetTaxMethodsByProviderKey(providerKey);
            foreach (var countryTaxRate in countryTaxRates)
            {
                TaxMethodService.Delete(countryTaxRate);
            }
        }

        /// <summary>
        /// Gets the <see cref="ITaxMethodService"/>
        /// </summary>
        internal ITaxMethodService TaxMethodService
        {
            get { return _serviceContext.TaxMethodService; }
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
            ((CustomerService)CustomerService).Save(customer);
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
            ((CustomerService)CustomerService).Save(customers);
            return customers;
        }

        /// <summary>
        /// Deletes all of the customers from the database
        /// </summary>
        public void DeleteAllCustomers()
        {
            var all = ((CustomerService) CustomerService).GetAll();
            ((CustomerService)CustomerService).Delete(all);
        }

        /// <summary>
        /// The customer service
        /// </summary>
        public ICustomerService CustomerService
        {
            get { return _serviceContext.CustomerService; }
        }

        /// <summary>
        /// Deletes all customer addresses
        /// </summary>
        public void DeleteAllCustomerAddresses()
        {
            var all = ((CustomerAddressService) CustomerAddressService).GetAll();
            ((CustomerAddressService)CustomerAddressService).Delete(all);
        }

        /// <summary>
        /// The customer address service
        /// </summary>
        public ICustomerAddressService CustomerAddressService
        {
            get { return _serviceContext.CustomerAddressService; }
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

        #region Notifications

        /// <summary>
        /// Deletes all notification methods
        /// </summary>
        public void DeleteAllNotificationMethods()
        {
            var methods = ((NotificationMethodService) NotificationMethodService).GetAll();
            NotificationMethodService.Delete(methods);
        }

        internal INotificationMethodService NotificationMethodService
        {
            get { return _serviceContext.NotificationMethodService; }
        }

        #endregion

        #region IOrder

        /// <summary>
        /// Deletes all orders
        /// </summary>
        public void DeleteAllOrders()
        {
            var all = ((OrderService) OrderService).GetAll();
            OrderService.Delete(all);
        }

        /// <summary>
        /// Gets the OrderService
        /// </summary>
        public IOrderService OrderService
        {
            get { return _serviceContext.OrderService; }
        }

        #endregion

        #region IProduct

        /// <summary>
        /// Saves a product record to the database and returns and instance of <see cref="IProduct"/> represents that record
        /// </summary>
        /// <returns><see cref="IProduct"/></returns>
        public IProduct MakeExistingProduct(bool shippable = true, decimal weight = 0, decimal price = 0, bool taxable = true)
        {
            var product = MockProductDataMaker.MockProductForInserting(shippable, weight, price, taxable);            
            ProductService.Save(product);
            product.AddToCatalogInventory(WarehouseCatalog);
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

        /// <summary>
        /// Returns the Payment Service
        /// </summary>
        public IPaymentService PaymentService
        {
            get { return _serviceContext.PaymentService; }
        }

        #endregion

        #region IPaymentMethod

        public void DeleteAllPaymentMethods()
        {
            var methods = PaymentMethodService.GetAll();

            foreach (var method in methods)
            {
                PaymentMethodService.Delete(method);    
            }            
        }

        /// <summary>
        /// Gets the payment method service
        /// </summary>
        internal IPaymentMethodService PaymentMethodService
        {
            get { return _serviceContext.PaymentMethodService; }
        }

        #endregion


        #region Shipping (IShipment, IShipCounty)


        public void DeleteAllShipments()
        {
            var all = ((ShipmentService) ShipmentService).GetAll();
            ShipmentService.Delete(all);
        }

        /// <summary>
        /// Returns the Shipping Service
        /// </summary>
        public IShipmentService ShipmentService
        {
            get
            {
                return _serviceContext.ShipmentService;
            }
        }

        #endregion

        #region ShipCountry

        public void DeleteAllShipCountries()
        {
            var shipCountries = ((ShipCountryService)ShipCountryService).GetAllShipCountries();
            foreach (var country in shipCountries.Where(x => !x.CountryCode.Equals(Constants.CountryCodes.EverywhereElse)))
            {
                ShipCountryService.Delete(country);
            }

        }

        public IShipCountryService ShipCountryService
        {
            get { return _serviceContext.ShipCountryService; }
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

        public void DeleteWarehouseCatalogs()
        {
            var catalogs = WarehouseService.GetAllWarehouseCatalogs();

            WarehouseService.Delete(catalogs);
        }

        /// <summary>
        /// Gets <see cref="IWarehouseService"/>
        /// </summary>
        public IWarehouseService WarehouseService
        {
            get { return _serviceContext.WarehouseService; } 

        }
        #endregion


        public void ValidateDatabaseSetup()
        {
            if (!Database.TableExist("merchGatewayProviderSettings"))
                RebuildDatabase();
    
            var providerDtos =  Database.Query<GatewayProviderSettingsDto>("SELECT * FROM merchGatewayProviderSettings");
            var warehouseDtos = Database.Query<WarehouseDto>("SELECT * FROM merchWarehouse");
            var catalogDtos =   Database.Query<WarehouseCatalogDto>("SELECT * FROM merchWarehouseCatalog");
            var typeFieldDtos = Database.Query<TypeFieldDto>("SELECT * FROM merchTypeField");
            var invoiceStatusDtos = Database.Query<InvoiceStatusDto>("SELECT * FROM merchInvoiceStatus");
            var orderStatusDtos = Database.Query<OrderStatusDto>("SELECT * FROM merchOrderStatus");
            var storeSettingDtos = Database.Query<StoreSettingDto>("SELECT * FROM merchStoreSetting");

            if (!providerDtos.Any() || !warehouseDtos.Any() || !catalogDtos.Any() || !typeFieldDtos.Any() || !invoiceStatusDtos.Any() || !orderStatusDtos.Any() || !storeSettingDtos.Any())
            {
                RebuildDatabase();
            }
            
        }

        private void RebuildDatabase()
        {
            // migration
            var schema = new DatabaseSchemaCreation(Database);

            // drop all the tables
            schema.UninstallDatabaseSchema();

            // install the schema
            schema.InitializeDatabaseSchema();

            // add the default data
            var baseDataCreation = new BaseDataCreation(Database);
            baseDataCreation.InitializeBaseData("merchDBTypeField");
            baseDataCreation.InitializeBaseData("merchInvoiceStatus");
            baseDataCreation.InitializeBaseData("merchOrderStatus");
            baseDataCreation.InitializeBaseData("merchWarehouse");
            baseDataCreation.InitializeBaseData("merchGatewayProviderSettings");
            baseDataCreation.InitializeBaseData("merchStoreSetting");
        }

    }
}
