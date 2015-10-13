namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;

    using Models;
    using Models.TypeFields;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the AppliedPaymentService
    /// </summary>
    internal class AppliedPaymentService : MerchelloRepositoryService, IAppliedPaymentService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPaymentService"/> class.
        /// </summary>
        public AppliedPaymentService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPaymentService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AppliedPaymentService(ILogger logger)
            : this(logger, new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPaymentService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public AppliedPaymentService(ILogger logger, RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPaymentService"/> class.
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
        public AppliedPaymentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPaymentService"/> class.
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
        public AppliedPaymentService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers



        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, Events.NewEventArgs<IAppliedPayment>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, Events.NewEventArgs<IAppliedPayment>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, SaveEventArgs<IAppliedPayment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, SaveEventArgs<IAppliedPayment>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAppliedPaymentService, DeleteEventArgs<IAppliedPayment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAppliedPaymentService, DeleteEventArgs<IAppliedPayment>> Deleted;

        #endregion

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
        public IAppliedPayment CreateAppliedPaymentWithKey(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType, string description, decimal amount, bool raiseEvents = true)
        {
            return CreateAppliedPaymentWithKey(
                paymentKey, 
                invoiceKey,
                EnumTypeFieldConverter.AppliedPayment.GetTypeField(appliedPaymentType).TypeKey, 
                description, 
                amount, 
                raiseEvents);
        }
       
        /// <summary>
        /// Saves an <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IAppliedPayment appliedPayment, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IAppliedPayment>(appliedPayment), this))
                {
                    ((AppliedPayment)appliedPayment).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    repository.AddOrUpdate(appliedPayment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAppliedPayment>(appliedPayment), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayments">The collection of <see cref="IAppliedPayment"/>s to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IAppliedPayment> appliedPayments, bool raiseEvents = true)
        {
            var paymentsArray = appliedPayments as IAppliedPayment[] ?? appliedPayments.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IAppliedPayment>(paymentsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    foreach (var appliedPayment in paymentsArray)
                    {
                        repository.AddOrUpdate(appliedPayment);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IAppliedPayment>(paymentsArray), this);
        }

        /// <summary>
        /// Deletes a <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayment">The <see cref="IAppliedPayment"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IAppliedPayment appliedPayment, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IAppliedPayment>(appliedPayment), this))
                {
                    ((AppliedPayment)appliedPayment).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    repository.Delete(appliedPayment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAppliedPayment>(appliedPayment), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IAppliedPayment"/>
        /// </summary>
        /// <param name="appliedPayments">The collection of <see cref="IAppliedPayment"/>s to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IAppliedPayment> appliedPayments, bool raiseEvents = true)
        {
            var payemntsArray = appliedPayments as IAppliedPayment[] ?? appliedPayments.ToArray();

            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IAppliedPayment>(payemntsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    foreach (var appliedPayment in payemntsArray)
                    {
                        repository.Delete(appliedPayment);    
                    }
                    
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IAppliedPayment>(payemntsArray), this);
        }

        /// <summary>
        /// Returns a <see cref="IAppliedPayment"/> by it's unique 'key'
        /// </summary>
        /// <param name="key">The unique 'key' of the <see cref="IAppliedPayment"/></param>
        /// <returns>An <see cref="IAppliedPayment"/></returns>
        public IAppliedPayment GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the payment key
        /// </summary>
        /// <param name="paymentKey">The payment key</param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public IEnumerable<IAppliedPayment> GetAppliedPaymentsByPaymentKey(Guid paymentKey)
        {
            using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IAppliedPayment>.Builder.Where(x => x.PaymentKey == paymentKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IAppliedPayment"/>s by the invoice key
        /// </summary>
        /// <param name="invoiceKey">The invoice key</param>
        /// <returns>A collection <see cref="IAppliedPayment"/></returns>
        public IEnumerable<IAppliedPayment> GetAppliedPaymentsByInvoiceKey(Guid invoiceKey)
        {
            using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IAppliedPayment>.Builder.Where(x => x.InvoiceKey == invoiceKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// The create applied payment with key.
        /// </summary>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="appliedPaymentTfKey">
        /// The applied payment tf key.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IAppliedPayment"/>.
        /// </returns>
        internal IAppliedPayment CreateAppliedPaymentWithKey(Guid paymentKey, Guid invoiceKey, Guid appliedPaymentTfKey, string description, decimal amount, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(paymentKey), "paymentKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(appliedPaymentTfKey), "appliedPaymentTfKey");

            var appliedPayment = new AppliedPayment(paymentKey, invoiceKey, appliedPaymentTfKey)
            {
                Description = description,
                Amount = amount,
                Exported = false
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IAppliedPayment>(appliedPayment), this))
                {
                    appliedPayment.WasCancelled = true;
                    return appliedPayment;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAppliedPaymentRepository(uow))
                {
                    repository.AddOrUpdate(appliedPayment);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IAppliedPayment>(appliedPayment), this);

            return appliedPayment;
        }

    }
}