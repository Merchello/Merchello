using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents a NotificationMethodService
    /// </summary>
    internal class NotificationMethodService : INotificationMethodService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public NotificationMethodService()
            : this(new RepositoryFactory())
        { }

        public NotificationMethodService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public NotificationMethodService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Creates a <see cref="INotificationMethod"/> and saves it to the database
        /// </summary>
        /// <param name="providerKey">The <see cref="IGatewayProviderSettings"/> key</param>
        /// <param name="name">The name of the notification (used in back office)</param>
        /// <param name="serviceCode">The notification service code</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>An Attempt{<see cref="INotificationMethod"/>}</returns>
        public Attempt<INotificationMethod> CreateNotificationMethodWithKey(Guid providerKey, string name, string serviceCode, bool raiseEvents = true)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(serviceCode, "serviceCode");

            var notificationMethod = new NotificationMethod(providerKey)
            {
                Name = name, 
                ServiceCode = serviceCode
            };

            if(raiseEvents)
            if(Creating.IsRaisedEventCancelled(new Events.NewEventArgs<INotificationMethod>(notificationMethod), this))
            {
                notificationMethod.WasCancelled = true;
                return Attempt<INotificationMethod>.Fail(notificationMethod);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMethodRepository(uow))
                {
                    repository.AddOrUpdate(notificationMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<INotificationMethod>(notificationMethod), this);

            return Attempt<INotificationMethod>.Succeed(notificationMethod);
        }

        /// <summary>
        /// Saves a single instance of <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethod">The <see cref="INotificationMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(INotificationMethod notificationMethod, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<INotificationMethod>(notificationMethod), this))
                {
                    ((NotificationMethod)notificationMethod).WasCancelled = true;
                    return;
                }
         
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMethodRepository(uow))
                {
                    repository.AddOrUpdate(notificationMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<INotificationMethod>(notificationMethod), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethods">The collection of <see cref="INotificationMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<INotificationMethod> notificationMethods, bool raiseEvents = true)
        {
            var notificationMethodsArray = notificationMethods as INotificationMethod[] ?? notificationMethods.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<INotificationMethod>(notificationMethodsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMethodRepository(uow))
                {
                    foreach (var notificationMethod in notificationMethodsArray)
                    {
                        repository.AddOrUpdate(notificationMethod);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<INotificationMethod>(notificationMethodsArray), this);
        }

        /// <summary>
        /// Deletes a single instance os <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethod">The <see cref="INotificationMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(INotificationMethod notificationMethod, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<INotificationMethod>(notificationMethod), this))
            {
                ((NotificationMethod) notificationMethod).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMethodRepository(uow))
                {
                    repository.Delete(notificationMethod);
                    uow.Commit();
                }
            }

            if(raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<INotificationMethod>(notificationMethod), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="INotificationMethod"/>
        /// </summary>
        /// <param name="notificationMethods">The collection of <see cref="INotificationMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<INotificationMethod> notificationMethods, bool raiseEvents = true)
        {
            var notificationMethodsArray = notificationMethods as INotificationMethod[] ?? notificationMethods.ToArray();
            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<INotificationMethod>(notificationMethodsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMethodRepository(uow))
                {
                    foreach (var notificationMethod in notificationMethodsArray)
                    {
                        repository.Delete(notificationMethod);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<INotificationMethod>(notificationMethodsArray), this);
        }

        /// <summary>
        /// Gets a <see cref="INotificationMethod"/> by it's key
        /// </summary>
        /// <param name="key">The key (Guid) of the <see cref="INotificationMethod"/> to be retrieved</param>
        /// <returns>The <see cref="INotificationMethod"/></returns>
        public INotificationMethod GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateNotificationMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="INotificationMethod"/> assoicated with a provider
        /// </summary>
        /// <param name="providerKey">The <see cref="IGatewayProviderSettings"/> key</param>
        /// <returns>A collection of all <see cref="INotificationMethod"/> associated with a provider</returns>
        public IEnumerable<INotificationMethod> GetNotifcationMethodsByProviderKey(Guid providerKey)
        {
            using(var repository = _repositoryFactory.CreateNotificationMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<INotificationMethod>.Builder.Where(x => x.ProviderKey == providerKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of all notification methods.  
        /// </summary>
        /// <remarks>Primarily used for testing</remarks>
        internal IEnumerable<INotificationMethod> GetAll()
        {
            using (var repository = _repositoryFactory.CreateNotificationMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<INotificationMethodService, Events.NewEventArgs<INotificationMethod>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<INotificationMethodService, Events.NewEventArgs<INotificationMethod>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<INotificationMethodService, SaveEventArgs<INotificationMethod>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<INotificationMethodService, SaveEventArgs<INotificationMethod>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<INotificationMethodService, DeleteEventArgs<INotificationMethod>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<INotificationMethodService, DeleteEventArgs<INotificationMethod>> Deleted;

        #endregion
    }
}