namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the PaymentMethodService
    /// </summary>
    internal class PaymentMethodService : MerchelloRepositoryService, IPaymentMethodService
    {
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodService"/> class.
        /// </summary>
        public PaymentMethodService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public PaymentMethodService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public PaymentMethodService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodService"/> class.
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
        public PaymentMethodService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodService"/> class.
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
        public PaymentMethodService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        /// <summary>
        /// Attempts to create a <see cref="IPaymentMethod"/> for a given provider.  If the provider already 
        /// defines a paymentCode, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        /// <param name="name">The name of the payment method</param>
        /// <param name="description">The description of the payment method</param>
        /// <param name="paymentCode">The unique 'payment code' associated with the payment method.  (Eg. visa, mc)</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="IPaymentMethod"/> with respective success or fail</returns>
        internal Attempt<IPaymentMethod> CreatePaymentMethodWithKey(Guid providerKey, string name, string description, string paymentCode, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(paymentCode, "paymentCode");

            if (GetPaymentMethodByPaymentCode(providerKey, paymentCode) != null) return Attempt<IPaymentMethod>.Fail(new ConstraintException("A PaymentMethod already exists for the provider for the paymentCode '" + paymentCode + "'"));

            var paymentMethod = new PaymentMethod(providerKey)
                {
                    Name = name,
                    Description = description,
                    PaymentCode = paymentCode
                };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IPaymentMethod>(paymentMethod), this))
                {
                    paymentMethod.WasCancelled = true;
                    return Attempt<IPaymentMethod>.Fail(paymentMethod);
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreatePaymentMethodRepository(uow))
                {
                    repository.AddOrUpdate(paymentMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IPaymentMethod>(paymentMethod), this);

            return Attempt<IPaymentMethod>.Succeed(paymentMethod);
        }

        /// <summary>
        /// Saves a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IPaymentMethod paymentMethod, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IPaymentMethod>(paymentMethod), this))
                {
                    ((PaymentMethod)paymentMethod).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreatePaymentMethodRepository(uow))
                {
                    repository.AddOrUpdate(paymentMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPaymentMethod>(paymentMethod), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="paymentMethods">A collection of <see cref="IPaymentMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IPaymentMethod> paymentMethods, bool raiseEvents = true)
        {
            var paymentMethodsArray = paymentMethods as IPaymentMethod[] ?? paymentMethods.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IPaymentMethod>(paymentMethodsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreatePaymentMethodRepository(uow))
                {
                    foreach (var paymentMethod in paymentMethodsArray)
                    {
                        repository.AddOrUpdate(paymentMethod);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IPaymentMethod>(paymentMethodsArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethod">The <see cref="IPaymentMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IPaymentMethod paymentMethod, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IPaymentMethod>(paymentMethod), this))
                {
                    ((PaymentMethod)paymentMethod).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreatePaymentMethodRepository(uow))
                {
                    repository.Delete(paymentMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IPaymentMethod>(paymentMethod), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="paymentMethods">The collection of <see cref="IPaymentMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IPaymentMethod> paymentMethods, bool raiseEvents = true)
        {
            var methods = paymentMethods as IPaymentMethod[] ?? paymentMethods.ToArray();
            if(raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IPaymentMethod>(methods), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreatePaymentMethodRepository(uow))
                {
                    foreach (var method in methods)
                    {
                        repository.Delete(method);
                    }
                    uow.Commit();
                }
            }

            if(raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<IPaymentMethod>(methods), this);
        }

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/>
        /// </summary>
        /// <param name="key">The unique 'key' (Guid) of the <see cref="IPaymentMethod"/></param>
        /// <returns><see cref="IPaymentMethod"/></returns>
        public IPaymentMethod GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreatePaymentMethodRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IPaymentMethod"/> for a given PaymentGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <returns>A collection of <see cref="IPaymentMethod"/></returns>
        public IEnumerable<IPaymentMethod> GetPaymentMethodsByProviderKey(Guid providerKey)
        {
            using (var repository = RepositoryFactory.CreatePaymentMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IPaymentMethod>.Builder.Where(x => x.ProviderKey == providerKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Returns a <see cref="IPaymentMethod"/> given is't paymentCode 
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the PaymentGatewayProvider</param>
        /// <param name="paymentCode">The paymentCode</param>
        public IPaymentMethod GetPaymentMethodByPaymentCode(Guid providerKey, string paymentCode)
        {
            using (var repository = RepositoryFactory.CreatePaymentMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<IPaymentMethod>.Builder.Where(
                        x => x.ProviderKey == providerKey && x.PaymentCode == paymentCode);

                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="IPaymentMethod"/>
        /// </summary>
        public IEnumerable<IPaymentMethod> GetAll()
        {
            using (var repository = RepositoryFactory.CreatePaymentMethodRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IPaymentMethodService, Events.NewEventArgs<IPaymentMethod>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IPaymentMethodService, Events.NewEventArgs<IPaymentMethod>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IPaymentMethodService, SaveEventArgs<IPaymentMethod>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IPaymentMethodService, SaveEventArgs<IPaymentMethod>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IPaymentMethodService, DeleteEventArgs<IPaymentMethod>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IPaymentMethodService, DeleteEventArgs<IPaymentMethod>> Deleted;

        #endregion
    }
}