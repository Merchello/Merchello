using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    public class GatewayProviderService : IGatewayProviderService
    {

        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IInvoiceService _invoiceService;
        private readonly IShipMethodService _shipMethodService;
        private readonly IShipRateTierService _shipRateTierService;
        private readonly IShipCountryService _shipCountryService;
        private readonly ITaxMethodService _taxMethodService;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentMethodService _paymentMethodService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public GatewayProviderService()
            : this(new RepositoryFactory(), new ShipMethodService(), new ShipRateTierService(), new ShipCountryService(), new InvoiceService(),  new TaxMethodService(), new PaymentService(),  new PaymentMethodService())
        { }

         internal GatewayProviderService(RepositoryFactory repositoryFactory, IShipMethodService shipMethodService, 
             IShipRateTierService shipRateTierService, IShipCountryService shipCountryService, 
             IInvoiceService invoiceService,
             ITaxMethodService taxMethodService, IPaymentService paymentService, IPaymentMethodService paymentMethodService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, shipMethodService, 
             shipRateTierService, shipCountryService, invoiceService, taxMethodService,
             paymentService, paymentMethodService)
        { }

        internal GatewayProviderService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, 
            IShipMethodService shipMethodService, IShipRateTierService shipRateTierService, 
            IShipCountryService shipCountryService, IInvoiceService invoiceService, ITaxMethodService taxMethodService, 
            IPaymentService paymentService, IPaymentMethodService paymentMethodService)
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

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _shipMethodService = shipMethodService;
            _shipRateTierService = shipRateTierService;
            _shipCountryService = shipCountryService;
            _invoiceService = invoiceService;
            _taxMethodService = taxMethodService;
            _paymentService = paymentService;
            _paymentMethodService = paymentMethodService;
        }


        #region GatewayProvider

        /// <summary>
        /// Saves a single instance of a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IGatewayProvider gatewayProvider, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IGatewayProvider>(gatewayProvider), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    repository.AddOrUpdate(gatewayProvider);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IGatewayProvider>(gatewayProvider), this);
            }
        }

        /// <summary>
        /// Deletes a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IGatewayProvider gatewayProvider, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProvider), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    repository.Delete(gatewayProvider);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProvider), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProviderList"></param>
        /// <param name="raiseEvents"></param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal void Delete(IEnumerable<IGatewayProvider> gatewayProviderList, bool raiseEvents = true)
        {
            var gatewayProviderArray = gatewayProviderList as IGatewayProvider[] ?? gatewayProviderList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProviderArray), this);

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

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProviderArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IGatewayProvider"/> by it's unique 'Key' (Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IGatewayProvider GetGatewayProviderByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/> by its type (Shipping, Taxation, Payment)
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        public IEnumerable<IGatewayProvider> GetGatewayProvidersByType(GatewayProviderType gatewayProviderType)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<IGatewayProvider>.Builder.Where(
                        x =>
                            x.ProviderTfKey ==
                            EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProviderType).TypeKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/> by ship country
        /// </summary>
        /// <param name="shipCountry"></param>
        /// <returns></returns>
        public IEnumerable<IGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetGatewayProvidersByShipCountryKey(shipCountry.Key);
            }
        }

        /// <summary>
        /// Gets a collection containing all <see cref="IGatewayProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGatewayProvider> GetAllGatewayProviders()
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
            return _paymentService.GetPaymentsForInvoice(invoiceKey);
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
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProvider"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        public IEnumerable<IShipMethod> GetShipMethodsByShipCountryKey(Guid providerKey, Guid shipCountryKey)
        {
            return _shipMethodService.GetGatewayProviderShipMethods(providerKey, shipCountryKey);
        }

        /// <summary>
        /// Gets a list of all <see cref="IShipMethod"/> objects given a <see cref="IGatewayProvider"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        public IEnumerable<IShipMethod> GetShipMethodsByShipCountryKey(Guid providerKey)
        {
            return _shipMethodService.GetGatewayProviderShipMethods(providerKey);
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

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the <see cref="IGatewayProvider"/></param>
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
        public static event TypedEventHandler<IGatewayProviderService, SaveEventArgs<IGatewayProvider>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, SaveEventArgs<IGatewayProvider>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IGatewayProviderService, DeleteEventArgs<IGatewayProvider>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, DeleteEventArgs<IGatewayProvider>> Deleted;

        #endregion
    }
}