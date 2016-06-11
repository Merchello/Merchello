namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
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
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents an anonymous customer service.
    /// </summary>
    internal class AnonymousCustomerService : MerchelloRepositoryService, IAnonymousCustomerService
    {
         #region fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        public AnonymousCustomerService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AnonymousCustomerService(ILogger logger)
            : this(logger, ApplicationContext.Current.DatabaseContext.SqlSyntax)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public AnonymousCustomerService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(logger, new RepositoryFactory(logger, sqlSyntax))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public AnonymousCustomerService(ILogger logger, RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
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
        public AnonymousCustomerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousCustomerService"/> class.
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
        public AnonymousCustomerService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers



        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, Events.NewEventArgs<IAnonymousCustomer>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, Events.NewEventArgs<IAnonymousCustomer>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, SaveEventArgs<IAnonymousCustomer>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, SaveEventArgs<IAnonymousCustomer>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAnonymousCustomerService, DeleteEventArgs<IAnonymousCustomer>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAnonymousCustomerService, DeleteEventArgs<IAnonymousCustomer>> Deleted;


        #endregion


        /// <summary>
        /// The create anonymous customer with key.
        /// </summary>
        /// <returns>
        /// The <see cref="IAnonymousCustomer"/>.
        /// </returns>
        public IAnonymousCustomer CreateAnonymousCustomerWithKey()
        {
            var anonymous = new AnonymousCustomer();

            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IAnonymousCustomer>(anonymous), this))
            {
                anonymous.WasCancelled = true;
                return anonymous;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.AddOrUpdate(anonymous);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<IAnonymousCustomer>(anonymous), this);

            return anonymous;
        }

        /// <summary>
        /// Saves the <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">
        /// The anonymous customer
        /// </param>
        /// <param name="raiseEvents">
        /// TOptional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IAnonymousCustomer anonymous, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IAnonymousCustomer>(anonymous), this))
            {
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.AddOrUpdate(anonymous);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Saved.RaiseEvent(new SaveEventArgs<IAnonymousCustomer>(anonymous), this);
        }

        /// <summary>
        /// Deletes the <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymous">
        /// The anonymous customer
        /// </param>
        public void Delete(IAnonymousCustomer anonymous)
        {
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IAnonymousCustomer>(anonymous), this)) return;

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    repository.Delete(anonymous);
                    uow.Commit();
                }
            }

            Deleted.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymous), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IAnonymousCustomer"/>
        /// </summary>
        /// <param name="anonymouses">
        /// The anonymous customers to be deleted
        /// </param>
        public void Delete(IEnumerable<IAnonymousCustomer> anonymouses)
        {
            var anonymousArray = anonymouses as IAnonymousCustomer[] ?? anonymouses.ToArray();

            Deleting.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymousArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();

                using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(uow))
                {
                    foreach (var anonymous in anonymousArray)
                    {
                        repository.Delete(anonymous);
                    }

                    uow.Commit();
                }
            }

            Deleted.RaiseEvent(new DeleteEventArgs<IAnonymousCustomer>(anonymousArray), this);
        }

        /// <summary>
        /// The get anonymous customers created before a certain date.
        /// </summary>
        /// <param name="createdDate">
        /// The created Date.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IAnonymousCustomer"/> older than a certain number of days.
        /// </returns>
        /// <remarks>
        /// For maintenance routines
        /// </remarks>
        public IEnumerable<IAnonymousCustomer> GetAnonymousCustomersCreatedBefore(DateTime createdDate)
        {
            using (var repository = RepositoryFactory.CreateAnonymousCustomerRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IAnonymousCustomer>.Builder.Where(x => x.CreateDate <= createdDate);

                return repository.GetByQuery(query);
            }
        }
    }
}