namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.UI;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    /// <summary>
    /// Represents the Customer Service, 
    /// </summary>
    public class CustomerService : PageCachedServiceBase<ICustomer>, ICustomerService
    {
        #region fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// The valid sort fields.
        /// </summary>
        private static readonly string[] ValidSortFields = { "firstname", "lastname", "loginname", "email", "lastactivitydate" };

        /// <summary>
        /// The anonymous customer service.
        /// </summary>
        private readonly IAnonymousCustomerService _anonymousCustomerService;

        /// <summary>
        /// The customer address service.
        /// </summary>
        private readonly ICustomerAddressService _customerAddressService;

        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The payment service.
        /// </summary>
        private readonly IPaymentService _paymentService;        

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        public CustomerService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CustomerService(ILogger logger)
            : this(new RepositoryFactory(), logger, new AnonymousCustomerService(logger), new CustomerAddressService(logger), new InvoiceService(logger), new PaymentService(logger)) 
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public CustomerService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(new RepositoryFactory(logger, sqlSyntax), logger, new AnonymousCustomerService(logger, sqlSyntax), new CustomerAddressService(logger, sqlSyntax), new InvoiceService(logger, sqlSyntax), new PaymentService(logger, sqlSyntax))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="anonymousCustomerService">
        /// The anonymous Customer Service.
        /// </param>
        /// <param name="customerAddressService">
        /// The customer Address Service.
        /// </param>
        /// <param name="invoiceService">
        /// The invoice Service.
        /// </param>
        /// <param name="paymentService">
        /// The payment Service.
        /// </param>
        public CustomerService(
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IAnonymousCustomerService anonymousCustomerService,
            ICustomerAddressService customerAddressService,
            IInvoiceService invoiceService,
            IPaymentService paymentService)
            : this(
            new PetaPocoUnitOfWorkProvider(logger),
            repositoryFactory,
            logger,
            anonymousCustomerService,
            customerAddressService,
            invoiceService,
            paymentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="anonymousCustomerService">
        /// The anonymous Customer Service.
        /// </param>
        /// <param name="customerAddressService">
        /// The customer Address Service.
        /// </param>
        /// <param name="invoiceService">
        /// The invoice Service.
        /// </param>
        /// <param name="paymentService">
        /// The payment Service.
        /// </param>
        public CustomerService(
            IDatabaseUnitOfWorkProvider provider,
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IAnonymousCustomerService anonymousCustomerService,
            ICustomerAddressService customerAddressService,
            IInvoiceService invoiceService,
            IPaymentService paymentService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), anonymousCustomerService, customerAddressService, invoiceService, paymentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        /// <param name="anonymousCustomerService">
        /// The anonymous customer service.
        /// </param>
        /// <param name="customerAddressService">
        /// The customer address service.
        /// </param>
        /// <param name="invoiceService">
        /// The invoice service.
        /// </param>
        /// <param name="paymentService">
        /// The payment service.
        /// </param>
        public CustomerService(
            IDatabaseUnitOfWorkProvider provider,
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory,
            IAnonymousCustomerService anonymousCustomerService,
            ICustomerAddressService customerAddressService,
            IInvoiceService invoiceService,
            IPaymentService paymentService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            Ensure.ParameterNotNull(anonymousCustomerService, "anonymousCustomerService");
            Ensure.ParameterNotNull(customerAddressService, "customerAddressService");
            Ensure.ParameterNotNull(invoiceService, "invoiceServie");
            Ensure.ParameterNotNull(paymentService, "paymentService");
            _anonymousCustomerService = anonymousCustomerService;
            _customerAddressService = customerAddressService;
            _invoiceService = invoiceService;
            _paymentService = paymentService;
        }

        #region Event Handlers



        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<ICustomerService, Events.NewEventArgs<ICustomer>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICustomerService, Events.NewEventArgs<ICustomer>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICustomerService, SaveEventArgs<ICustomer>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICustomerService, SaveEventArgs<ICustomer>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICustomerService, DeleteEventArgs<ICustomer>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICustomerService, DeleteEventArgs<ICustomer>> Deleted;



        #endregion


        #region ICustomerService Members

        /// <summary>
        /// Creates a customer without saving to the database
        /// </summary>
        /// <param name="loginName">The login name of the customer.</param>
        /// <param name="firstName">The first name of the customer</param>
        /// <param name="lastName">The last name of the customer</param>
        /// <param name="email">the email address of the customer</param>
        /// <returns>The <see cref="ICustomer"/></returns>
        public ICustomer CreateCustomer(string loginName, string firstName, string lastName, string email)
        {
            Ensure.ParameterNotNullOrEmpty(loginName, "loginName");
            var customer = new Customer(loginName)
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

            if (!Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICustomer>(customer), this))
            {
                return customer;
            }

            customer.WasCancelled = true;
            
            return customer;
        }

        /// <summary>
        /// Creates a customer and saves the record to the database
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <param name="firstName">
        /// The first name of the customer
        /// </param>
        /// <param name="lastName">
        /// The last name of the customer
        /// </param>
        /// <param name="email">
        /// the email address of the customer
        /// </param>
        /// <returns>
        /// <see cref="ICustomer"/>
        /// </returns>
        public ICustomer CreateCustomerWithKey(string loginName, string firstName, string lastName, string email)
        {
            Ensure.ParameterNotNullOrEmpty(loginName, "loginName");

            var customer = new Customer(loginName)
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICustomer>(customer), this))
            {
                customer.WasCancelled = true;
                return customer;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerRepository(uow))
                {
                    repository.AddOrUpdate(customer);
                    uow.Commit();
                }
            }

            SaveAddresses(customer);

            Created.RaiseEvent(new Events.NewEventArgs<ICustomer>(customer), this);

            return customer;
        }

        /// <summary>
        /// Creates a customer with the Umbraco login name
        /// </summary>
        /// <param name="loginName">
        /// The login Name.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>
        /// </returns>
        public ICustomer CreateCustomerWithKey(string loginName)
        {
            return CreateCustomerWithKey(loginName, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Saves a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(ICustomer customer, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomer>(customer), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerRepository(uow))
                {
                    repository.AddOrUpdate(customer);
                    uow.Commit();
                }                
            }

            SaveAddresses(customer);

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomer>(customer), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="ICustomer"/> objects.
        /// </summary>
        /// <param name="customers">Collection of <see cref="ICustomer"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICustomer> customers, bool raiseEvents = true)
        {
            var customerArray = customers as ICustomer[] ?? customers.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICustomer>(customerArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                
                using (var repository = RepositoryFactory.CreateCustomerRepository(uow))
                {
                    foreach (var customer in customerArray)
                    {
                        repository.AddOrUpdate(customer);
                    }
          
                    uow.Commit();
                }               
            }

            foreach (var customer in customerArray)
            {
                SaveAddresses(customer);
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICustomer>(customerArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICustomer"/> object
        /// </summary>
        /// <param name="customer">The <see cref="ICustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICustomer customer, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomer>(customer), this);
          
            DeleteInvoicesAndPayments(customer);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerRepository(uow))
                {
                    repository.Delete(customer);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomer>(customer), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICustomer"/> objects
        /// </summary>
        /// <param name="customers">Collection of <see cref="ICustomer"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ICustomer> customers, bool raiseEvents = true)
        {
            var customerArray = customers as ICustomer[] ?? customers.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICustomer>(customerArray), this);

            foreach (var customer in customerArray)
            {
                DeleteInvoicesAndPayments(customer);
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateCustomerRepository(uow))
                {
                    foreach (var customer in customerArray)
                    {
                        repository.Delete(customer);
                    }

                    uow.Commit();                    
                }                
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICustomer>(customerArray), this);
        }

        /// <summary>
        /// Gets a customer by its unique id
        /// </summary>
        /// <param name="key">GUID key for the customer</param>
        /// <returns><see cref="ICustomer"/></returns>
        public override ICustomer GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a page of <see cref="ICustomer"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Icustomer}"/>.
        /// </returns>
        public override Page<ICustomer> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<ICustomer>.Builder.Where(x => x.Key != Guid.Empty);

                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }
       
        /// <summary>
        /// Gets an <see cref="ICustomer"/> or <see cref="IAnonymousCustomer"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="entityKey">GUID key of either object to retrieve</param>
        /// <returns><see cref="ICustomerBase"/></returns>
        public ICustomerBase GetAnyByKey(Guid entityKey)
        {
            ICustomerBase customer;

            // try retrieving an anonymous customer first as in most situations this will be what is being queried
            using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                customer = repository.Get(entityKey);
            }

            if (customer != null) return customer;

            // try retrieving an existing customer
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(entityKey);
            }
        }

        /// <summary>
        /// The get by login name.
        /// </summary>
        /// <param name="loginName">
        /// The login name.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        public ICustomer GetByLoginName(string loginName)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<ICustomer>.Builder.Where(x => x.LoginName == loginName);

                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the total customer count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CustomerCount()
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<ICustomer>.Builder.Where(x => x.Key != Guid.Empty);

                return repository.Count(query);
            }
        }

        /// <summary>
        /// Creates an <see cref="IAnonymousCustomer"/> and saves it to the database
        /// </summary>
        /// <returns><see cref="IAnonymousCustomer"/></returns>
        public IAnonymousCustomer CreateAnonymousCustomerWithKey()
        {
            return _anonymousCustomerService.CreateAnonymousCustomerWithKey();
        }

        /// <summary>
        /// Saves a single <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">
        /// The <see cref="IAnonymousCustomer"/> to save
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IAnonymousCustomer anonymous, bool raiseEvents = true)
        {
            _anonymousCustomerService.Save(anonymous, raiseEvents);
        }


        /// <summary>
        /// Deletes a single <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">The <see cref="IAnonymousCustomer"/> to delete</param>
        public void Delete(IAnonymousCustomer anonymous)
        {
            _anonymousCustomerService.Delete(anonymous);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IAnonymousCustomer"/> objects
        /// </summary>
        /// <param name="anonymouses">Collection of <see cref="IAnonymousCustomer"/> to delete</param>
        public void Delete(IEnumerable<IAnonymousCustomer> anonymouses)
        {
            _anonymousCustomerService.Delete(anonymouses);
        }

        #region Customer Address

        /// <summary>
        /// Saves a single <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address to be saved
        /// </param>
        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public void Save(ICustomerAddress address)
        {
            _customerAddressService.Save(address);
        }

        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public void Save(IEnumerable<ICustomerAddress> addresses)
        {
            _customerAddressService.Save(addresses);
        }

        /// <summary>
        /// Deletes a single instance of the <see cref="ICustomerAddress"/>
        /// </summary>
        /// <param name="address">
        /// The address to be deleted
        /// </param>
        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public void Delete(ICustomerAddress address)
        {
            _customerAddressService.Delete(address);
        }

        /// <summary>
        /// Gets an address by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public ICustomerAddress GetAddressByKey(Guid key)
        {
            return _customerAddressService.GetByKey(key);
        }

        /// <summary>
        /// Gets a collection of <see cref="ICustomerAddress"/> by the customer key
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey)
        {
            return _customerAddressService.GetByCustomerKey(customerKey);
        }

        /// <summary>
        /// Gets a collection of <see cref="ICustomerAddress"/> by the customer key filtered by an <see cref="AddressType"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// A collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public IEnumerable<ICustomerAddress> GetByCustomerKey(Guid customerKey, AddressType addressType)
        {
            return _customerAddressService.GetByCustomerKey(customerKey, addressType);
        }

        /// <summary>
        /// Gets the default customer address of a certain type
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="addressType">
        /// The address type.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        [Obsolete("Addresses should be managed directly through the customer.  This method will be removed in future versions")]
        public ICustomerAddress GetDefaultCustomerAddress(Guid customerKey, AddressType addressType)
        {
            return _customerAddressService.GetDefaultCustomerAddress(customerKey, addressType);
        }

        #endregion

        /// <summary>
        /// Gets a list of customer give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns>A collection of <see cref="ICustomer"/></returns>
        public IEnumerable<ICustomer> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }


        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="entity">
        /// The customer.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void AddToCollection(ICustomer entity, IEntityCollection collection)
        {
            AddToCollection(entity, collection.Key);
        }

        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="entity">
        /// The customer.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(ICustomer entity, Guid collectionKey)
        {
            AddToCollection(entity.Key, collectionKey);
        }

        /// <summary>
        /// The add invoice to collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(Guid entityKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                repository.AddToCollection(entityKey, collectionKey);
            }
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entity">
        /// The customer.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public void RemoveFromCollection(ICustomer entity, IEntityCollection collection)
        {
            RemoveFromCollection(entity, collection.Key);
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entity">
        /// The customer.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(ICustomer entity, Guid collectionKey)
        {
            RemoveFromCollection(entity.Key, collectionKey);
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entityKey">
        /// The customer key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(Guid entityKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                repository.RemoveFromCollection(entityKey, collectionKey);
            }
        }

        /// <summary>
        /// Determines if an customer exists in a collection.
        /// </summary>
        /// <param name="entityKey">
        /// The customer key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, Guid collectionKey)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.ExistsInCollection(entityKey, collectionKey);
            }
        }

        /// <summary>
        /// Returns true if the entity exists in the at least one of the static collections.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, IEnumerable<Guid> collectionKeys)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.ExistsInCollection(entityKey, collectionKeys.ToArray());
            }
        }

        /// <summary>
        /// The get invoices from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<ICustomer> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetFromCollection(
                    collectionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets distinct customers from multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<ICustomer> GetProductsThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetEntitiesThatExistInAllCollections(
                    collectionKeys.ToArray(),
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        public Page<ICustomer> GetFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetFromCollection(
                    collectionKey,
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// Gets distinct filtered customers from multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        public Page<ICustomer> GetProductsThatExistInAllCollections(
            IEnumerable<Guid> collectionKeys,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetEntitiesThatExistInAllCollections(
                    collectionKeys.ToArray(),
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get customer keys from static collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysFromCollection(
                    collectionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysFromCollection(
                    collectionKey,
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysNotInCollection(
                    collectionKey,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        internal Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetKeysNotInCollection(
                    collectionKey,
                    searchTerm,
                    page,
                    itemsPerPage,
                    this.ValidateSortByField(sortBy),
                    sortDirection);
            }
        }

        #endregion

        /// <summary>
        /// For testing
        /// </summary>
        /// <returns>
        /// The collection of all anonymous customers
        /// </returns>
        internal IEnumerable<IAnonymousCustomer> GetAllAnonymousCustomers()
        {
            using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }
      

        /// <summary>
        /// For testing.
        /// </summary>
        /// <returns>
        /// The collection of all customers.
        /// </returns>
        internal IEnumerable<ICustomer> GetAll()
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets a count of items returned by a query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal override int Count(IQuery<ICustomer> query)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Count(query);
            }
        }

        /// <summary>
        /// Gets a <see cref="Page{Guid}"/>
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> GetPagedKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repositoy = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<ICustomer>.Builder.Where(x => x.Key != Guid.Empty);

                return repositoy.GetPagedKeys(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Gets a page by search term
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        /// <remarks>
        /// The search is prefabricated in the repository
        /// </remarks>
        internal Page<Guid> GetPagedKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.SearchKeys(searchTerm, page, itemsPerPage, ValidateSortByField(sortBy));
            }
        }    

        /// <summary>
        /// Gets a page by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        internal Page<Guid> GetPagedKeys(
            IQuery<ICustomer> query,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetPagedKeys(
                RepositoryFactory.CreateCustomerRepository(UowProvider.GetUnitOfWork()),
                query,
                page,
                itemsPerPage,
                ValidateSortByField(sortBy),
                sortDirection);
        }

        /// <summary>
        /// Validates the sort by field
        /// </summary>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string ValidateSortByField(string sortBy)
        {
            return ValidSortFields.Contains(sortBy.ToLower()) ? sortBy : "loginName";
        }

        /// <summary>
        /// The save addresses.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        private void SaveAddresses(ICustomer customer)
        {
            if (!customer.Addresses.Any()) return;

            AssertAddressCount(customer);
            var addresses = customer.Addresses.ToArray();

            for (int i = 0; i < addresses.Count(); i++)
            {
                if (addresses[i].CustomerKey.Equals(Guid.Empty))
                {
                    ((CustomerAddress)addresses[i]).CustomerKey = customer.Key;
                }

                _customerAddressService.Save(addresses[i]);
            }
        }

        /// <summary>
        /// Assert the customer address count.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        private void AssertAddressCount(ICustomer customer)
        {
            var count = _customerAddressService.Count(customer.Key);
            if (count == customer.Addresses.Count()) return;
            var deletes =
                _customerAddressService.GetByCustomerKey(customer.Key)
                    .Where(x => customer.Addresses.All(y => y.Key != x.Key));
            ((CustomerAddressService)_customerAddressService).Delete(deletes);
        }

        /// <summary>
        /// Deletes invoices and payments associated with a customer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <remarks>
        /// This helps clean up the Examine (Lucene) indexes
        /// </remarks>
        private void DeleteInvoicesAndPayments(ICustomer customer)
        {
            var invoices = _invoiceService.GetInvoicesByCustomerKey(customer.Key).ToArray();
            if (invoices.Any()) _invoiceService.Delete(invoices);

            var payments = _paymentService.GetPaymentsByCustomerKey(customer.Key).ToArray();
            if (payments.Any()) _paymentService.Delete(payments);
        }
    }
}