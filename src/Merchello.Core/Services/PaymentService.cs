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
    /// Represents the PaymentService
    /// </summary>
    public class PaymentService : IPaymentService
    {
        #region Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// The applied payment service.
        /// </summary>
        private readonly IAppliedPaymentService _appliedPaymentService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        public PaymentService()
            : this(new AppliedPaymentService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="appliedPaymentService">
        /// The applied payment service.
        /// </param>
        internal PaymentService(IAppliedPaymentService appliedPaymentService)
            : this(new RepositoryFactory(), appliedPaymentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="appliedPaymentService">
        /// The applied payment service.
        /// </param>
        internal PaymentService(RepositoryFactory repositoryFactory, IAppliedPaymentService appliedPaymentService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, appliedPaymentService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="appliedPaymentService">
        /// The applied payment service.
        /// </param>
        internal PaymentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IAppliedPaymentService appliedPaymentService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(appliedPaymentService, "appliedPaymentService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _appliedPaymentService = appliedPaymentService;
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IPaymentService, Events.NewEventArgs<IPayment>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IPaymentService, Events.NewEventArgs<IPayment>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IPaymentService, SaveEventArgs<IPayment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IPaymentService, SaveEventArgs<IPayment>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IPaymentService, DeleteEventArgs<IPayment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IPaymentService, DeleteEventArgs<IPayment>> Deleted;

        #endregion

        /// <summary>
        /// Creates a payment without saving it to the database
        /// </summary>
        /// <param name="paymentMethodType">The type of the payment method</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional payment method Key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        public IPayment CreatePayment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, bool raiseEvents = true)
        {
            var payment = new Payment(paymentMethodType, amount, paymentMethodKey);

            if(raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IPayment>(payment), this))
            {
                payment.WasCancelled = true;
                return payment;
            }

            if (raiseEvents) 
            Created.RaiseEvent(new Events.NewEventArgs<IPayment>(payment), this);

            return payment;

        }

        /// <summary>
        /// Creates and saves a payment
        /// </summary>
        /// <param name="paymentMethodType">The type of the payment method</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional payment Method Key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        public IPayment CreatePaymentWithKey(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, bool raiseEvents = true)
        {
            return CreatePaymentWithKey(EnumTypeFieldConverter.PaymentMethod.GetTypeField(paymentMethodType).TypeKey, amount, paymentMethodKey);
        }



        /// <summary>
        /// Saves a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IPayment payment, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IPayment>(payment), this))
                {
                    ((Payment)payment).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    repository.AddOrUpdate(payment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPayment>(payment), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IPayment"/>
        /// </summary>
        /// <param name="payments">A collection of <see cref="IPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IPayment> payments, bool raiseEvents = true)
        {
            var paymentsArray = payments as IPayment[] ?? payments.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IPayment>(paymentsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    foreach (var paymentMethod in paymentsArray)
                    {
                        repository.AddOrUpdate(paymentMethod);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPayment>(paymentsArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IPayment payment, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IPayment>(payment), this))
                {
                    ((Payment)payment).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    repository.Delete(payment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPayment>(payment), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IPayment"/>
        /// </summary>
        /// <param name="payments">
        /// The payments.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<IPayment> payments, bool raiseEvents = true)
        {
            var paymentsArray = payments as IPayment[] ?? payments.ToArray();
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IPayment>(paymentsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();

                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    foreach (var payment in paymentsArray)
                    {
                        repository.Delete(payment);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPayment>(paymentsArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IPayment"/>
        /// </summary>
        /// <param name="key">The unique 'key' (GUID) of the <see cref="IPayment"/></param>
        /// <returns><see cref="IPaymentMethod"/></returns>
        public IPayment GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets list of <see cref="IProduct"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of GUID keys for Product objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        public IEnumerable<IPayment> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> for a given PaymentGatewayProvider
        /// </summary>
        /// <param name="paymentMethodKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        public IEnumerable<IPayment> GetPaymentsByPaymentMethodKey(Guid? paymentMethodKey)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IPayment>.Builder.Where(x => x.PaymentMethodKey == paymentMethodKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> for a given invoice
        /// </summary>
        /// <param name="invoiceKey">The unique 'key' of the invoice</param>
        /// <returns>A collection of <see cref="IPayment"/></returns>
        public IEnumerable<IPayment> GetPaymentsByInvoiceKey(Guid invoiceKey)
        {
            var paymentKeys = _appliedPaymentService.GetAppliedPaymentsByInvoiceKey(invoiceKey).Select(x => x.PaymentKey).Distinct().ToArray();
            
            return !paymentKeys.Any() ? new List<IPayment>() : GetByKeys(paymentKeys);
        }

        /// <summary>
        /// Gets a collection of <see cref="IPayment"/> by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IPayment"/>.
        /// </returns>
        public IEnumerable<IPayment> GetPaymentsByCustomerKey(Guid customerKey)
        {
            using (var repository = _repositoryFactory.CreatePaymentRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IPayment>.Builder.Where(x => x.CustomerKey == customerKey);

                return repository.GetByQuery(query);
            }
        }

        #region AppliedPayments
        

        /// <summary>
        /// Creates and saves an AppliedPayment
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <param name="invoiceKey">The invoice 'key'</param>
        /// <param name="appliedPaymentType">The applied payment type</param>
        /// <param name="description">The description of the payment application</param>
        /// <param name="amount">The amount of the payment to be applied</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>An <see cref="IAppliedPayment"/></returns>
        public IAppliedPayment ApplyPaymentToInvoice(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType, string description, decimal amount, bool raiseEvents = true)
        {
            return _appliedPaymentService.CreateAppliedPaymentWithKey(paymentKey, invoiceKey, appliedPaymentType, description, amount, raiseEvents);
        }

        /// <summary>
        /// Saves an <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IAppliedPayment appliedPayment, bool raiseEvents = true)
        {
            _appliedPaymentService.Save(appliedPayment, raiseEvents);
        }

        /// <summary>
        /// Deletes a <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IAppliedPayment appliedPayment, bool raiseEvents = true)
        {
            _appliedPaymentService.Delete(appliedPayment, raiseEvents);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayments">The collection of <see cref="IAppliedPayment"/>s to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IAppliedPayment> appliedPayments, bool raiseEvents = true)
        {
            _appliedPaymentService.Delete(appliedPayments, raiseEvents);
        }

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the payment key
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public IEnumerable<IAppliedPayment> GetAppliedPaymentsByPaymentKey(Guid paymentKey)
        {
            return _appliedPaymentService.GetAppliedPaymentsByPaymentKey(paymentKey);
        }

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the invoice key
        /// </summary>
        /// <param name="invoiceKey">The invoice key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public IEnumerable<IAppliedPayment> GetAppliedPaymentsByInvoiceKey(Guid invoiceKey)
        {
            return _appliedPaymentService.GetAppliedPaymentsByInvoiceKey(invoiceKey);
        }

        #endregion

        /// <summary>
        /// Creates and saves a payment
        /// </summary>
        /// <param name="paymentTfKey">The payment typefield key</param>
        /// <param name="amount">The amount of the payment</param>
        /// <param name="paymentMethodKey">The optional paymentMethodKey</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Returns <see cref="IPayment"/></returns>
        internal IPayment CreatePaymentWithKey(Guid paymentTfKey, decimal amount, Guid? paymentMethodKey, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(paymentTfKey), "paymentTfKey");


            var payment = new Payment(paymentTfKey, amount, paymentMethodKey, new ExtendedDataCollection());

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IPayment>(payment), this))
                {
                    payment.WasCancelled = true;
                    return payment;
                }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreatePaymentRepository(uow))
                {
                    repository.AddOrUpdate(payment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IPayment>(payment), this);

            return payment;
        }
    }
}