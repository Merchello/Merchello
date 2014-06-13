using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Represents a NotificationMessageService
    /// </summary>
    internal class NotificationMessageService : INotificationMessageService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public NotificationMessageService()
            : this(new RepositoryFactory())
        { }

        public NotificationMessageService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public NotificationMessageService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
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
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Attempt{INotificationMessage}</returns>
        public Attempt<INotificationMessage> CreateNotificationMethodWithKey(Guid methodKey, string name, string description, string fromAddress,
            IEnumerable<string> recipients, string bodyText, bool raiseEvents = true)
        {
            var recipientArray = recipients as string[] ?? recipients.ToArray();

            Mandate.ParameterCondition(methodKey != Guid.Empty, "methodKey");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(fromAddress, "fromAddress");            

            var message = new NotificationMessage(methodKey, name, fromAddress)
            {
                Description = description,
                BodyText = bodyText,
                Recipients = string.Join(",", recipientArray)
            };

            if(raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<INotificationMessage>(message), this))
            {
                message.WasCancelled = true;
                return Attempt<INotificationMessage>.Fail(message);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMessageRepository(uow))
                {
                    repository.AddOrUpdate(message);
                    uow.Commit();
                }
            }

            if(raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<INotificationMessage>(message), this);

            return Attempt<INotificationMessage>.Succeed(message);
        }

        /// <summary>
        /// Saves a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="INotificationMessage"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(INotificationMessage notificationMessage, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<INotificationMessage>(notificationMessage), this))
            {
                ((NotificationMessage) notificationMessage).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMessageRepository(uow))
                {

                    repository.AddOrUpdate(notificationMessage);
             
                    uow.Commit();
                }
            }

            if (raiseEvents) 
            Saved.RaiseEvent(new SaveEventArgs<INotificationMessage>(notificationMessage), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="INotificationMessage"/>s
        /// </summary>
        /// <param name="notificationMessages">The collection of <see cref="INotificationMessage"/>s to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<INotificationMessage> notificationMessages, bool raiseEvents = true)
        {
            var notificationMessagesArray = notificationMessages as INotificationMessage[] ?? notificationMessages.ToArray();

            if(raiseEvents)
            Saving.RaiseEvent(new SaveEventArgs<INotificationMessage>(notificationMessagesArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMessageRepository(uow))
                {
                    foreach (var notificationMessage in notificationMessagesArray)
                    {
                        repository.AddOrUpdate(notificationMessage);
                    }                    
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Saved.RaiseEvent(new SaveEventArgs<INotificationMessage>(notificationMessagesArray), this);
        }

        /// <summary>
        /// Deletes a single instance of <see cref="INotificationMessage"/>
        /// </summary>
        /// <param name="notificationMessage">The <see cref="INotificationMessage"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(INotificationMessage notificationMessage, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<INotificationMessage>(notificationMessage), this))
            {
                ((NotificationMessage) notificationMessage).WasCancelled = true;
                return;
            }
            
             using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNotificationMessageRepository(uow))
                {
                    repository.Delete(notificationMessage);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<INotificationMessage>(notificationMessage), this);        
        }

        /// <summary>
        /// Gets a <see cref="INotificationMessage"/> by it's unique key (Guid)
        /// </summary>
        /// <param name="key">The key (Guid) for the <see cref="INotificationMessage"/> to be retrieved</param>
        public INotificationMessage GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateNotificationMessageRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

		/// <summary>
		/// Gets a collection of <see cref="INotificationMessage"/> by list of unique key (Guid)
		/// </summary>
		/// <param name="keys">The keys (Guid) for the collection of <see cref="INotificationMessage"/> to be retrieved</param>
		public IEnumerable<INotificationMessage> GetByKeys(IEnumerable<Guid> keys)
		{
			using (var repository = _repositoryFactory.CreateNotificationMessageRepository(_uowProvider.GetUnitOfWork()))
			{
				return repository.GetAll(keys.ToArray());
			}
		}

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s base on the notification method
        /// </summary>
        /// <param name="notificationMethodKey">The <see cref="INotificationMethod"/> key</param>
        /// <returns>Optional boolean indicating whether or not to raise events</returns>
        public IEnumerable<INotificationMessage> GetNotificationMessagesByMethodKey(Guid notificationMethodKey)
        {
            using (var repostiory = _repositoryFactory.CreateNotificationMessageRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<INotificationMessage>.Builder.Where(x => x.MethodKey == notificationMethodKey);

                return repostiory.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/>s based on a monitor key
        /// </summary>
        /// <param name="monitorKey">The Notification Monitor Key (Guid)</param>
        /// <returns>A collection of <see cref="INotificationMessage"/></returns>        
        public IEnumerable<INotificationMessage> GetNotificationMessagesByMonitorKey(Guid monitorKey)
        {
            using (var repository = _repositoryFactory.CreateNotificationMessageRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<INotificationMessage>.Builder.Where(x => x.MonitorKey == monitorKey);

                return repository.GetByQuery(query);
            }
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<INotificationMessageService, Events.NewEventArgs<INotificationMessage>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<INotificationMessageService, Events.NewEventArgs<INotificationMessage>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<INotificationMessageService, SaveEventArgs<INotificationMessage>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<INotificationMessageService, SaveEventArgs<INotificationMessage>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<INotificationMessageService, DeleteEventArgs<INotificationMessage>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<INotificationMessageService, DeleteEventArgs<INotificationMessage>> Deleted;

        #endregion
    }
}