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
    /// Represents the NoteService
    /// </summary>
    public class NoteService : PageCachedServiceBase<INote>, INoteService
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
        /// Initializes a new instance of the <see cref="NoteService"/> class.
        /// </summary>
        public NoteService()
            : this(new PetaPocoUnitOfWorkProvider(), new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The <see cref="IDatabaseUnitOfWorkProvider"/>
        /// </param>
        /// <param name="repositoryFactory">
        /// The <see cref="RepositoryFactory"/>
        /// </param>
        public NoteService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
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
        public static event TypedEventHandler<INoteService, Events.NewEventArgs<INote>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<INoteService, Events.NewEventArgs<INote>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<INoteService, SaveEventArgs<INote>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<INoteService, SaveEventArgs<INote>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<INoteService, DeleteEventArgs<INote>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<INoteService, DeleteEventArgs<INote>> Deleted;



        #endregion

        /// <summary>
        /// Gets an <see cref="INote"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>  
        public override INote GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateNoteRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }        

        /// <summary>
        /// Creates a note and saves it to the database
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        public INote CreateNoteWithKey(string message, bool raiseEvents = true)
        {
            return CreateNoteWithKey(null, null, message, raiseEvents);
        }


        /// <summary>
        /// Creates a note and saves it to the database
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
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        public INote CreateNoteWithKey(Guid? entityKey, EntityType entityType, string message, bool raiseEvents = true)
        {
            return CreateNoteWithKey(entityKey, EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey, message, raiseEvents);
        }

        /// <summary>
        /// Creates a note and saves it to the database
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
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="INote"/>.
        /// </returns>
        public INote CreateNoteWithKey(Guid? entityKey, Guid? entityTfKey, string message, bool raiseEvents = true)
        {
            var note = new Note()
            {
                EntityKey = entityKey,
                EntityTfKey = entityTfKey,
                Message = message
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<INote>(note), this))
            {
                note.WasCancelled = true;
                return note;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNoteRepository(uow))
                {
                    repository.AddOrUpdate(note);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<INote>(note), this);

            return note;
        }

        
        /// <summary>
        /// Saves an <see cref="INote"/>
        /// </summary>
        /// <param name="note">
        /// The <see cref="INote"/> to save
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(INote note, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<INote>(note), this))
            {
                ((Note)note).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNoteRepository(uow))
                {
                    repository.AddOrUpdate(note);
                    uow.Commit();
                }
            }

            Saved.RaiseEvent(new SaveEventArgs<INote>(note), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="INote"/>
        /// </summary>
        /// <param name="notes">
        /// The collection of <see cref="INote"/>s to be saved
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IEnumerable<INote> notes, bool raiseEvents = true)
        {
            var notesArray = notes as INote[] ?? notes.ToArray();

            if (raiseEvents)
                Saving.RaiseEvent(new SaveEventArgs<INote>(notesArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNoteRepository(uow))
                {
                    foreach (var note in notesArray)
                    {
                        repository.AddOrUpdate(note);    
                    }

                    uow.Commit();                        
                }                
            }

            Saved.RaiseEvent(new SaveEventArgs<INote>(notesArray), this);
        }

        /// <summary>
        /// Deletes a <see cref="INote"/>
        /// </summary>
        /// <param name="note">
        /// The <see cref="INote"/> to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(INote note, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<INote>(note), this))
            {
                ((Note)note).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNoteRepository(uow))
                {
                    repository.Delete(note);
                    uow.Commit();
                }
            }

            Deleted.RaiseEvent(new DeleteEventArgs<INote>(note), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="INote"/>
        /// </summary>
        /// <param name="notes">
        /// The collection of <see cref="INote"/>s to be deleted
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(IEnumerable<INote> notes, bool raiseEvents = true)
        {
            var notesArray = notes as INote[] ?? notes.ToArray();

            if (raiseEvents)
                Deleting.RaiseEvent(new DeleteEventArgs<INote>(notesArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateNoteRepository(uow))
                {
                    foreach (var note in notesArray)
                    {
                        repository.Delete(note);
                    }
                    uow.Commit();
                }
            }

            Deleted.RaiseEvent(new DeleteEventArgs<INote>(notesArray), this);
        }

      
        /// <summary>
        /// Gets a collection of <see cref="INote"/> for a particular entity
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INote}"/>.
        /// </returns>        
        public IEnumerable<INote> GetNotesByEntityKey(Guid entityKey)
        {
            using (var repository = _repositoryFactory.CreateNoteRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<INote>.Builder.Where(x => x.EntityKey == entityKey);

                return repository.GetByQuery(query);
            }
        }


        /// <summary>
        /// Gets a collection of <see cref="INote"/> for an entity type
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
        /// The <see cref="Page{INote}"/>.
        /// </returns>
        public Page<INote> GetNotesByEntityTfKey(Guid entityTfKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = _repositoryFactory.CreateNoteRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Persistence.Querying.Query<INote>.Builder.Where(x => x.EntityTfKey == entityTfKey);

                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }


        /// <summary>
        /// Gets a <see cref="Page{INote}"/>
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
        public override Page<INote> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Persistence.Querying.Query<INote>.Builder.Where(x => x.Key != Guid.Empty);

            using (var repository = _repositoryFactory.CreateNoteRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
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
        internal override int Count(IQuery<INote> query)
        {
            using (var repository = _repositoryFactory.CreateNoteRepository(_uowProvider.GetUnitOfWork()))
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
            var query = Persistence.Querying.Query<INote>.Builder.Where(x => x.Key != Guid.Empty);
            return GetPagedKeys(
                _repositoryFactory.CreateNoteRepository(_uowProvider.GetUnitOfWork()),
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