namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents the GatewayProviderService
    /// </summary>    
    public class GatewayProviderService : IGatewayProviderService
    {
        //TODO - we are adding so many services here, we should consider refactoring GatewayProviderBase to 
        // TODO simply accept the ServiceContext
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IInvoiceService _invoiceService;
        private readonly IOrderService _orderService;
        private readonly IShipMethodService _shipMethodService;
        private readonly IShipRateTierService _shipRateTierService;
        private readonly IShipCountryService _shipCountryService;
        private readonly ITaxMethodService _taxMethodService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly INotificationMethodService _notificationMethodService;
        private readonly INotificationMessageService _notificationMessageService;
        private readonly IWarehouseService _warehouseService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         /// <summary>
         /// Constructor
         /// </summary>
         public GatewayProviderService()
            : this(new RepositoryFactory(), new ShipMethodService(), new ShipRateTierService(), new ShipCountryService(), new InvoiceService(), new OrderService(), new TaxMethodService(), new PaymentService(),  new PaymentMethodService(), new NotificationMethodService(), new NotificationMessageService(), new WarehouseService())
        { }

         internal GatewayProviderService(RepositoryFactory repositoryFactory, IShipMethodService shipMethodService, 
             IShipRateTierService shipRateTierService, IShipCountryService shipCountryService, 
             IInvoiceService invoiceService, IOrderService orderService,
             ITaxMethodService taxMethodService, IPaymentService paymentService, IPaymentMethodService paymentMethodService,
             INotificationMethodService notificationMethodService, INotificationMessageService notificationMessageService, IWarehouseService warehouseService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, shipMethodService, 
             shipRateTierService, shipCountryService, invoiceService, orderService, taxMethodService,
             paymentService, paymentMethodService,
             notificationMethodService, notificationMessageService, warehouseService)
        { }

        internal GatewayProviderService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, 
            IShipMethodService shipMethodService, IShipRateTierService shipRateTierService, 
            IShipCountryService shipCountryService, 
            IInvoiceService invoiceService, 
            IOrderService orderService, 
            ITaxMethodService taxMethodService, 
            IPaymentService paymentService, 
            IPaymentMethodService paymentMethodService, 
            INotificationMethodService notificationMethodService, 
            INotificationMessageService notificationMessageService,
            IWarehouseService warehouseService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(shipMethodService, "shipMethodService");
            Mandate.ParameterNotNull(shipRateTierService, "shipRateTierService");
            Mandate.ParameterNotNull(shipCountryService, "shipCountryService");
            Mandate.ParameterNotNull(taxMethodService, "countryTaxRateService");
            Mandate.ParameterNotNull(paymentService, "paymentService");
            Mandate.ParameterNotNull(paymentMethodService, "paymentMethodService");
            Mandate.ParameterNotNull(invoiceService, "invoiceService");
            Mandate.ParameterNotNull(orderService, "orderService");
            Mandate.ParameterNotNull(notificationMethodService, "notificationMethodService");
            Mandate.ParameterNotNull(notificationMessageService, "notificationMessageService");
            Mandate.ParameterNotNull(warehouseService, "warehouseService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _shipMethodService = shipMethodService;
            _shipRateTierService = shipRateTierService;
            _shipCountryService = shipCountryService;
            _invoiceService = invoiceService;
            _orderService = orderService;
            _taxMethodService = taxMethodService;
            _paymentService = paymentService;
            _paymentMethodService = paymentMethodService;
            _notificationMethodService = notificationMethodService;
            _notificationMessageService = notificationMessageService;
            _warehouseService = warehouseService;
        }


        #region Event Handlers

        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IGatewayProviderService, Events.NewEventArgs<IGatewayProvider>> Creating;


        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IGatewayProviderService, Events.NewEventArgs<IGatewayProvider>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, SaveEventArgs<IGatewayProviderSettings>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, SaveEventArgs<IGatewayProviderSettings>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IGatewayProviderService, DeleteEventArgs<IGatewayProviderSettings>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, DeleteEventArgs<IGatewayProviderSettings>> Deleted;

        #endregion

        #region GatewayProviderSettings

        /// <summary>
        /// Saves a single instance of a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IGatewayProviderSettings gatewayProviderSettings, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IGatewayProviderSettings>(gatewayProviderSettings), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    repository.AddOrUpdate(gatewayProviderSettings);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IGatewayProviderSettings>(gatewayProviderSettings), this);
            }
        }

        /// <summary>
        /// Deletes a <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderSettings"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IGatewayProviderSettings gatewayProviderSettings, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IGatewayProviderSettings>(gatewayProviderSettings), this);

            // delete associated methods
            switch (gatewayProviderSettings.GatewayProviderType)
            {
                case GatewayProviderType.Payment:
                    var paymentMethods = _paymentMethodService.GetPaymentMethodsByProviderKey(gatewayProviderSettings.Key).ToArray();
                    if(paymentMethods.Any()) _paymentMethodService.Delete(paymentMethods);
                    break;
                    
                case GatewayProviderType.Shipping:
                    var shippingMethods = _shipMethodService.GetShipMethodsByProviderKey(gatewayProviderSettings.Key).ToArray();
                    if(shippingMethods.Any()) _shipMethodService.Delete(shippingMethods);
                    break;

                case GatewayProviderType.Taxation:
                    var taxMethods = _taxMethodService.GetTaxMethodsByProviderKey(gatewayProviderSettings.Key).ToArray();
                    if(taxMethods.Any()) _taxMethodService.Delete(taxMethods);
                    break;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    repository.Delete(gatewayProviderSettings);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IGatewayProviderSettings>(gatewayProviderSettings), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <param name="gatewayProviderList"></param>
        /// <param name="raiseEvents"></param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal void Delete(IEnumerable<IGatewayProviderSettings> gatewayProviderList, bool raiseEvents = true)
        {
            var gatewayProviderArray = gatewayProviderList as IGatewayProviderSettings[] ?? gatewayProviderList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IGatewayProviderSettings>(gatewayProviderArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    foreach (var gatewayProvider in gatewayProviderArray)
                    {
                        repository.Delete(gatewayProvider);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IGatewayProviderSettings>(gatewayProviderArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IGatewayProviderSettings"/> by it's unique 'Key' (Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IGatewayProviderSettings GetGatewayProviderByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProviderSettings"/> by its type (Shipping, Taxation, Payment)
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        public IEnumerable<IGatewayProviderSettings> GetGatewayProvidersByType(GatewayProviderType gatewayProviderType)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<IGatewayProviderSettings>.Builder.Where(
                        x =>
                            x.ProviderTfKey ==
                            EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProviderType).TypeKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProviderSettings"/> by ship country
        /// </summary>
        /// <param name="shipCountry"></param>
        /// <returns></returns>
        public IEnumerable<IGatewayProviderSettings> GetGatewayProvidersByShipCountry(IShipCountry shipCountry)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetGatewayProvidersByShipCountryKey(shipCountry.Key);
            }
        }

        /// <summary>
        /// Gets a collection containing all <see cref="IGatewayProviderSettings"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGatewayProviderSettings> GetAllGatewayProviders()
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        #endregion

        #region AppliedPayments

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the payment key
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public IEnumerable<IAppliedPayment> GetAppliedPaymentsByPaymentKey(Guid paymentKey)
        {
            return _paymentService.GetAppliedPaymentsByPaymentKey(paymentKey);
        }

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the invoice key
        /// </summary>
        /// <param name="invoiceKey">The invoice key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public IEnumerable<IAppliedPayment> GetAppliedPaymentsByInvoiceKey(Guid invoiceKey)
        {
            return _paymentService.GetAppliedPaymentsByInvoiceKey(invoiceKey);
        }

        /// <summary>
        /// Saves a single <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be saved</param>
        public void Save(IAppliedPayment appliedPayment)
        {
            _paymentService.Save(appliedPayment);
        }

        #endregion

        #region Invoice

        /// <summary>
        /// Saves a single <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to save</param>
        public void Save(IInvoice invoice)
        {
            _invoiceService.Save(invoice);
        }


        #endregion

        #region PaymentMethod

        /// <summary>
        /// Attempts to create a <see cref="IPaymentMethod"/> for a given provider.  If the provider already 
        /// defines a paymentCode, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <param name="paymentCode">The unique 'payment code' associated with the payment method.  (Eg. visa, mc)</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="IPaymentMethod"/> with respective success or fail</returns>
        public Attempt<IPaymentMethod> CreatePaymentMethodWithKey(Guid providerKey, string name, string description, string paymentCode)
        {
            return ((PaymentMethodService)_paymentMethodService).CreatePaymentMethodWithKey(providerKey, name, description, paymentCode);
        }

        /// <summary>
        /// Saves a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be saved</param>        
        public void Save(IPaymentMethod paymentMethod)
        {
            _paymentMethodService.Save(paymentMethod);
        }

        /// <summary>
        /// Deletes a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be deleted</param> 
        public void Delete(IPaymentMethod paymentMethod)
        {
            _paymentMethodService.Delete(paymentMethod);
        }

        /// <summary>
        /// Gets a collection of <see cref="IPaymentMethod"/> for a given PaymentGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <returns>A collection of <see cref="IPaymentMethod"/></returns>
        public IEnumerable<IPaymentMethod> GetPaymentMethodsByProviderKey(Guid providerKey)
        {
            return _paymentMethodService.GetPaymentMethodsByProviderKey(providerKey);
        }


        #endregion

        #region Payment

        /// <summary>
        /// Creates a payment without saving it to the database
        /// </summary>
        /// <param name="paymentMethodType">The type of the paymentmethod</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        public IPayment CreatePayment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey)
        {
            return _paymentService.CreatePayment(paymentMethodType, amount, paymentMethodKey);
        }


        /// <summary>
        /// Creates and saves a payment
        /// </summary>
        /// <param name="paymentMethodType">The type of the paymentmethod</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        public IPayment CreatePaymentWithKey(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey)
        {
            return _paymentService.CreatePaymentWithKey(paymentMethodType, amount, paymentMethodKey);
        }

        /// <summary>
        /// Saves a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be saved</param>
        public void Save(IPayment payment)
        {
            _paymentService.Save(payment);
        }

        /// <summary>
        /// Creates and saves an AppliedPayment
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <param name="invoiceKey">The invoice 'key'</param>
        /// <param name="appliedPaymentType">The applied payment type</param>
        /// <param name="description">The description of the payment application</param>
        /// <param name="amount">The amount of the payment to be applied</param>
        /// <returns>An <see cref="IAppliedPayment"/></returns>
        public IAppliedPayment ApplyPaymentToInvoice(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType, string description, decimal amount)
        {
            return _paymentService.ApplyPaymentToInvoice(paymentKey, invoiceKey, appliedPaymentType, description, amount);
        }

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> for a given invoice
        /// </summary>
        /// <param name="invoiceKey">The unique 'key' of the invoice</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        public IEnumerable<IPayment> GetPaymentsForInvoice(Guid invoiceKey)
        {
            return _paymentService.GetPaymentsByInvoiceKey(invoiceKey);
        }


        #endregion


        #region Notification

        /// <summary>
        /// Creates a <see cref="INotificationMethod"/> and saves it to the database
        /// </summary>
        /// <param name="providerKey">The <see cref="IGatewayProviderSettings"/> key</param>
        /// <param name="name">The name of the notification (used in back office)</param>
        /// <param name="serviceCode">The notification service code</param>
        /// <returns>An Attempt{<see cref="INotificationMethod"/>}</returns>
        public Attempt<INotificationMethod> CreateNotificationMethodWithKey(Guid providerKey, string name, string serviceCode)
        {
            return _notificationMethodService.CreateNotificationMethodWithKey(providerKey, name, serviceCode);
        }

        /// <summary>
        /// Saves a <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="INotificationMethod"/> to be saved</param>
        public void Save(INotificationMethod method)
        {
            _notificationMethodService.Save(method);
        }

        /// <summary>
        /// Deletes a <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="method">The <see cref="INotificationMethod"/> to be deleted</param>
        public void Delete(INotificationMethod method)
        {
            _notificationMethodService.Delete(method);
        }

        /// <summary>
        /// Creates a <see cref="INotificationMessage"/> and saves it to the database
        /// </summary>
        /// <param name="methodKey">The <see cref="INotificationMethod"/> key</param>
        /// <param name="name">The name of the message (primarily used in the back office UI)</param>
        /// <param name="description">The name of the message (primarily used in the back office UI)</param>
        /// <param name="fromAddress">The senders or "from" address</param>
        /// <param name="recipients">A collection of recipient address</param>
        /// <param name="bodyText">The body text of the message</param>
        /// <returns>Attempt{INotificationMessage}</returns>
        public Attempt<INotificationMessage> CreateNotificationMessageWithKey(Guid methodKey, string name, string description, string fromAddress,
            IEnumerable<string> recipients, string bodyText)
        {
            return _notificationMessageService.CreateNotificationMethodWithKey(methodKey, name, description, fromAddress, recipients, bodyText);
        }

        /// <summary>
        /// Saves a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to save</param>
        public void Save(INotificationMessage message)
        {
            _notificationMessageService.Save(message);
        }

        /// <summary>
        /// Deletes a <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="message">The <see cref="INotificationMessage"/> to be deleted</param>
        public void Delete(INotificationMessage message)
        {
            _notificationMessageService.Delete(message);
        }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMethod"/> for a give NotificationGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the NotificationGatewayProvider</param>
        /// <returns>A collection of <see cref="INotificationMethod"/></returns>
        public IEnumerable<INotificationMethod> GetNotificationMethodsByProviderKey(Guid providerKey)
        {
            return _notificationMethodService.GetNotifcationMethodsByProviderKey(providerKey);
        }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/> associated with a <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethodKey">The key (Guid) of the <see cref="INotificationMethod"/></param>
        /// <returns>A collection of <see cref="INotificationMessage"/></returns>
        public IEnumerable<INotificationMessage> GetNotificationMessagesByMethodKey(Guid notificationMethodKey)
        {
            return _notificationMessageService.GetNotificationMessagesByMethodKey(notificationMethodKey);
        }

        /// <summary>
        /// Gets a <see cref="INotificationMethod"/> by it's unique key(Guid)
        /// </summary>
        /// <param name="notificationMessageKey">The unique key (Guid) of the <see cref="INotificationMessage"/></param>
        /// <returns>A <see cref="INotificationMessage"/></returns>
        public INotificationMessage GetNotificationMessageByKey(Guid notificationMessageKey)
        {
            return _notificationMessageService.GetByKey(notificationMessageKey);
        }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s based on a monitor key
        /// </summary>
        /// <param name="monitorKey">The Notification Monitor Key (Guid)</param>
        /// <returns>A collection of <see cref="INotificationMessage"/></returns>
        public IEnumerable<INotificationMessage> GetNotificationMessagesByMonitorKey(Guid monitorKey)
        {
            return _notificationMessageService.GetNotificationMessagesByMonitorKey(monitorKey);
        }

        #endregion

        #region ShipMethod

        /// <summary>
        /// Creates a <see cref="IShipMethod"/>.  This is useful due to the data constraint
        /// preventing two ShipMethods being created with the same ShipCountry and ServiceCode for any provider.
        /// </summary>
        /// <param name="providerKey">The unique gateway provider key (Guid)</param>
        /// <param name="shipCountry">The <see cref="IShipCountry"/> this ship method is to be associated with</param>
        /// <param name="name">The required name of the <see cref="IShipMethod"/></param>
        /// <param name="serviceCode">The ShipMethods service code</param>
        public Attempt<IShipMethod> CreateShipMethodWithKey(Guid providerKey, IShipCountry shipCountry, string name, string serviceCode)
        {            
            return ((ShipMethodService)_shipMethodService).CreateShipMethodWithKey(providerKey, shipCountry, name, serviceCode);
        }

        /// <summary>
        /// Saves a single <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        public void Save(IShipMethod shipMethod)
        {
           _shipMethodService.Save(shipMethod);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/></param>
        public void Save(IEnumerable<IShipMethod> shipMethodList)
        {
            _shipMethodService.Save(shipMethodList);
        }

        

        /// <summary>
        /// Deletes a <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        public void Delete(IShipMethod shipMethod)
        {
            _shipMethodService.Delete(shipMethod);
        }

        /// <summary>
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProviderSettings"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        public IEnumerable<IShipMethod> GetShipMethodsByShipCountryKey(Guid providerKey, Guid shipCountryKey)
        {
            return _shipMethodService.GetShipMethodsByProviderKey(providerKey, shipCountryKey);
        }

        /// <summary>
        /// Gets a list of all <see cref="IShipMethod"/> objects given a <see cref="IGatewayProviderSettings"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        public IEnumerable<IShipMethod> GetShipMethodsByShipCountryKey(Guid providerKey)
        {
            return _shipMethodService.GetShipMethodsByProviderKey(providerKey);
        }

        /// <summary>
        /// Gets a <see cref="IShipMethod"/> by it's unique key
        /// </summary>
        /// <param name="shipMethodKey">The <see cref="IShipMethod"/> key</param>
        /// <returns>A <see cref="IShipMethod"/></returns>
        public IShipMethod GetShipMethodByKey(Guid shipMethodKey)
        {
            return _shipMethodService.GetByKey(shipMethodKey);
        }


        #endregion

        #region ShipRateTier

        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void Save(IShipRateTier shipRateTier)
        {
            _shipRateTierService.Save(shipRateTier);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList"></param>
        public void Save(IEnumerable<IShipRateTier> shipRateTierList)
        {
            _shipRateTierService.Save(shipRateTierList);
        }


        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void Delete(IShipRateTier shipRateTier)
        {
            _shipRateTierService.Delete(shipRateTier);
        }


        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">Guid</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            return _shipRateTierService.GetShipRateTiersByShipMethodKey(shipMethodKey);
        }


        #endregion

        #region ShipCountry

        /// <summary>
        /// Gets a <see cref="IShipCountry"/> by it's unique key (Guid)
        /// </summary>
        /// <param name="shipCountryKey">The unique key of the <see cref="IShipCountry"/></param>
        /// <returns>The <see cref="IShipCountry"/></returns>
        public IShipCountry GetShipCountryByKey(Guid shipCountryKey)
        {
            return _shipCountryService.GetByKey(shipCountryKey);
        }

        /// <summary>
        /// Gets a <see cref="IShipCountry"/> by CatalogKey and CountryCode
        /// </summary>
        /// <param name="catalogKey">The unique key of the <see cref="IWarehouseCatalog"/></param>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <returns>An <see cref="IShipCountry"/></returns>
        public IShipCountry GetShipCountry(Guid catalogKey, string countryCode)
        {
            return _shipCountryService.GetShipCountryByCountryCode(catalogKey, countryCode);
        }

        /// <summary>
        /// Returns a collection of all <see cref="IShipCountry"/>
        /// </summary>
        /// <returns>A collection of all <see cref="IShipCountry"/></returns>
        public IEnumerable<IShipCountry> GetAllShipCountries()
        {
            return ((ShipCountryService) _shipCountryService).GetAllShipCountries();
        }


        /// <summary>
        /// Returns a collection of all <see cref="IInvoiceStatus"/>
        /// </summary>
        public IEnumerable<IInvoiceStatus> GetAllInvoiceStatuses()
        {
            return _invoiceService.GetAllInvoiceStatuses();
        }

        /// <summary>
        /// Returns a collection of all <see cref="IOrderStatus"/>
        /// </summary>
        public IEnumerable<IOrderStatus> GetAllOrderStatuses()
        {
            return _orderService.GetAllOrderStatuses();
        }

        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="ITaxMethod"/> with respective success or fail</returns>
        public Attempt<ITaxMethod> CreateTaxMethodWithKey(Guid providerKey, string countryCode, decimal percentageTaxRate)
        {
            return ((TaxMethodService)_taxMethodService).CreateTaxMethodWithKey(providerKey, countryCode, percentageTaxRate);
        }


#endregion

        #region TaxMethod
        
        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the <see cref="IGatewayProviderSettings"/></param>
        /// <param name="countryCode">The country code of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        public ITaxMethod GetTaxMethodByCountryCode(Guid providerKey, string countryCode)
        {
            return _taxMethodService.GetTaxMethodByCountryCode(providerKey, countryCode);
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="countryCode">The country code of the <see cref="ITaxMethod"/></param>
        /// <returns>A collection <see cref="ITaxMethod"/></returns>
        public IEnumerable<ITaxMethod> GetTaxMethodsByCountryCode(string countryCode)
        {
            return _taxMethodService.GetTaxMethodsByCountryCode(countryCode);
        }

        /// <summary>
        /// Saves a single <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be saved</param>
        public void Save(ITaxMethod taxMethod)
        {
            _taxMethodService.Save(taxMethod);
        }

        /// <summary>
        /// Deletes a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be deleted</param>
        public void Delete(ITaxMethod taxMethod)
        {
            _taxMethodService.Delete(taxMethod);
        }

        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> for a given TaxationGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the TaxationGatewayProvider</param>
        /// <returns>A collection of <see cref="ITaxMethod"/></returns>
        public IEnumerable<ITaxMethod> GetTaxMethodsByProviderKey(Guid providerKey)
        {
            return _taxMethodService.GetTaxMethodsByProviderKey(providerKey);
        }

        
        #endregion

        #region Warehouse

        public IWarehouse GetDefaultWarehouse()
        {
            return _warehouseService.GetDefaultWarehouse();
        }


        #endregion

    }
}