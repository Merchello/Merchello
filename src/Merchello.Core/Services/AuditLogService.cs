namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Models;
    using Models.Interfaces;
    using Models.TypeFields;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using RepositoryFactory = Persistence.RepositoryFactory;

    /// <summary>
    /// Represents the AuditLogService
    /// </summary>
    public class AuditLogService : PageCachedServiceBase<IAuditLog>, IAuditLogService
    {
        #region Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The valid sort fields.
        /// </summary>
        private static readonly string[] ValidSortFields = { "createdate" };

        /// <summary>
        /// The unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogService"/> class.
        /// </summary>
        public AuditLogService()
            : this(new PetaPocoUnitOfWorkProvider(), new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The <see cref="IDatabaseUnitOfWorkProvider"/>
        /// </param>
        /// <param name="repositoryFactory">
        /// The <see cref="RepositoryFactory"/>
        /// </param>
        public AuditLogService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region Event Handlers

        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IAuditLogService, Events.NewEventArgs<IAuditLog>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IAuditLogService, Events.NewEventArgs<IAuditLog>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IAuditLogService, SaveEventArgs<IAuditLog>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IAuditLogService, SaveEventArgs<IAuditLog>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IAuditLogService, DeleteEventArgs<IAuditLog>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IAuditLogService, DeleteEventArgs<IAuditLog>> Deleted;



        #endregion

        /// <summary>
        /// Gets an <see cref="IAuditLog"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>  
        public override IAuditLog GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }        

        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        public IAuditLog CreateAuditLogWithKey(string message, bool isError = false, bool raiseEvents = true)
        {
            return CreateAuditLogWithKey(message, new ExtendedDataCollection(), isError, raiseEvents);
        }

        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        public IAuditLog CreateAuditLogWithKey(string message, ExtendedDataCollection extendedData, bool isError = false, bool raiseEvents = true)
        {
            return CreateAuditLogWithKey(null, null, message, extendedData, isError, raiseEvents);
        }

        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="isError">
        /// The is error.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        public IAuditLog CreateAuditLogWithKey(Guid? entityKey, EntityType entityType, string message, bool isError = false, bool raiseEvents = true)
        {
            return CreateAuditLogWithKey(entityKey, EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey, message, new ExtendedDataCollection(), isError, raiseEvents);
        }

        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="isError">
        /// The is error.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        public IAuditLog CreateAuditLogWithKey(Guid? entityKey, EntityType entityType, string message, ExtendedDataCollection extendedData, bool isError = false, bool raiseEvents = true)
        {
            return CreateAuditLogWithKey(entityKey, EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey, message, extendedData, isError, raiseEvents);
        }        

        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        public IAuditLog CreateAuditLogWithKey(Guid? entityKey, Guid? entityTfKey, string message, ExtendedDataCollection extendedData, bool isError = false, bool raiseEvents = true)
        {
            var auditLog = new AuditLog()
            {
                EntityKey = entityKey,
                EntityTfKey = entityTfKey,
                Message = message,
                ExtendedData = extendedData,
                IsError = isError
            };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IAuditLog>(auditLog), this))
            {
                auditLog.WasCancelled = true;
                return auditLog;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAuditLogRepository(uow))
                {
                    repository.AddOrUpdate(auditLog);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<IAuditLog>(auditLog), this);

            return auditLog;
        }

        /// <summary>
        /// Saves an <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLog">
        /// The <see cref="IAuditLog"/> to save
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IAuditLog auditLog, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IAuditLog>(auditLog), this))
            {
                ((AuditLog)auditLog).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAuditLogRepository(uow))
                {
                    repository.AddOrUpdate(auditLog);
                    uow.Commit();
                }
            }
            
            Saved.RaiseEvent(new SaveEventArgs<IAuditLog>(auditLog), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLogs">
        /// The collection of <see cref="IAuditLog"/>s to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IEnumerable<IAuditLog> auditLogs, bool raiseEvents = true)
        {
            var auditLogsArray = auditLogs as IAuditLog[] ?? auditLogs.ToArray();

            if (raiseEvents)
            Saving.RaiseEvent(new SaveEventArgs<IAuditLog>(auditLogsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAuditLogRepository(uow))
                {
                    foreach (var auditLog in auditLogsArray)
                    {
                        repository.AddOrUpdate(auditLog);    
                    }

                    uow.Commit();                        
                }                
            }

            Saved.RaiseEvent(new SaveEventArgs<IAuditLog>(auditLogsArray), this);
        }

        /// <summary>
        /// Deletes a <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLog">
        /// The <see cref="IAuditLog"/> to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(IAuditLog auditLog, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IAuditLog>(auditLog), this))
            {
                ((AuditLog)auditLog).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAuditLogRepository(uow))
                {
                    repository.Delete(auditLog);
                    uow.Commit();
                }
            }

            Deleted.RaiseEvent(new DeleteEventArgs<IAuditLog>(auditLog), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IAuditLog"/>
        /// </summary>
        /// <param name="auditLogs">
        /// The collection of <see cref="IAuditLog"/>s to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(IEnumerable<IAuditLog> auditLogs, bool raiseEvents = true)
        {
            var auditLogsArray = auditLogs as IAuditLog[] ?? auditLogs.ToArray();

            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IAuditLog>(auditLogsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateAuditLogRepository(uow))
                {
                    foreach (var auditLog in auditLogsArray)
                    {
                        repository.Delete(auditLog);
                    }
                    uow.Commit();
                }
            }

            Deleted.RaiseEvent(new DeleteEventArgs<IAuditLog>(auditLogsArray), this);
        }

        /// <summary>
        /// Deletes all error logs
        /// </summary>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void DeleteErrorAuditLogs(bool raiseEvents = true)
        {
            Delete(GetErrorAuditLogs(1, int.MaxValue).Items, raiseEvents);
        }

        /// <summary>
        /// Gets a collection of <see cref="IAuditLog"/> for a particular entity
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IAuditLog}"/>.
        /// </returns>        
        public IEnumerable<IAuditLog> GetAuditLogsByEntityKey(Guid entityKey)
        {
            using (var repository = _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IAuditLog>.Builder.Where(x => x.EntityKey == entityKey);

                return repository.GetByQuery(query);
            }
        }


        /// <summary>
        /// Gets a collection of <see cref="IAuditLog"/> for an entity type
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
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
        /// The <see cref="Page{IAuditLog}"/>.
        /// </returns>
        public Page<IAuditLog> GetAuditLogsByEntityTfKey(Guid entityTfKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IAuditLog>.Builder.Where(x => x.EntityTfKey == entityTfKey);

                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IAuditLog"/> where IsError == true
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
        /// The <see cref="Page{IAuditLog}"/>.
        /// </returns>
        public Page<IAuditLog> GetErrorAuditLogs(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<IAuditLog>.Builder.Where(x => x.IsError);

                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Gets a <see cref="Page{IAuditLog}"/>
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
        /// The <see cref="Page{TEntity}"/>.
        /// </returns>
        public override Page<IAuditLog> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Persistence.Querying.Query<IAuditLog>.Builder.Where(x => x.Key != Guid.Empty);

            using (var repository = _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }


        /// <summary>
        /// Creates an audit record and saves it to the database
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="isError">
        /// Designates whether or not this is an error log record
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IAuditLog"/>.
        /// </returns>
        internal IAuditLog CreateAuditLogWithKey(Guid? entityKey, Guid? entityTfKey, string message, bool isError = false, bool raiseEvents = true)
        {
            return CreateAuditLogWithKey(entityKey, entityTfKey, message, new ExtendedDataCollection(), isError, raiseEvents);
        }

        /// <summary>
        /// Gets the count of items that would be returned by the query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal override int Count(IQuery<IAuditLog> query)
        {
            using (var repository = _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Count(query);
            }
        }

        /// <summary>
        /// The get paged keys.
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
        internal override Page<Guid> GetPagedKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Persistence.Querying.Query<IAuditLog>.Builder.Where(x => x.Key != Guid.Empty);
            return GetPagedKeys(
                _repositoryFactory.CreateAuditLogRepository(_uowProvider.GetUnitOfWork()),
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
            return ValidSortFields.Contains(sortBy.ToLower()) ? sortBy : "createdate";
        }
    }
}