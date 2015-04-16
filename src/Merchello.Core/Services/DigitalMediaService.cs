namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a <see cref="IDigitalMediaService"/>.
    /// </summary>
    public class DigitalMediaService : IDigitalMediaService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The database unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
        /// </summary>
        public DigitalMediaService()
            : this(new RepositoryFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public DigitalMediaService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public DigitalMediaService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, Events.NewEventArgs<IDigitalMedia>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, Events.NewEventArgs<IDigitalMedia>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, SaveEventArgs<IDigitalMedia>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, SaveEventArgs<IDigitalMedia>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IDigitalMediaService, DeleteEventArgs<IDigitalMedia>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, DeleteEventArgs<IDigitalMedia>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="IDigitalMedia"/> and saves it to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        public IDigitalMedia CreateDigitalMediaWithKey(string name, bool raiseEvents = true)
        {
            var digitalMedia = new DigitalMedia() { Name = name };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IDigitalMedia>(digitalMedia), this))
            {
                digitalMedia.WasCancelled = true;
                return digitalMedia;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    repository.AddOrUpdate(digitalMedia);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IDigitalMedia>(digitalMedia), this);

            return digitalMedia;
        }

        /// <summary>
        /// Saves a single <see cref="IDigitalMedia"/>
        /// </summary>
        /// <param name="digitalMedia">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IDigitalMedia digitalMedia, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMedia), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    repository.AddOrUpdate(digitalMedia);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMedia), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IDigitalMedia"/>.
        /// </summary>
        /// <param name="digitalMedias">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IEnumerable<IDigitalMedia> digitalMedias, bool raiseEvents = true)
        {
            var digitalMediaArray = digitalMedias as IDigitalMedia[] ?? digitalMedias.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMediaArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();

                using (var repository = _repositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    foreach (var digitalMedia in digitalMediaArray)
                    {
                        repository.AddOrUpdate(digitalMedia);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMediaArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IDigitalMedia"/> from the database.
        /// </summary>
        /// <param name="digitalMedia">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IDigitalMedia digitalMedia, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMedia), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    repository.Delete(digitalMedia);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMedia), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IDigitalMedia"/> from the database.
        /// </summary>
        /// <param name="digitalMedias">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<IDigitalMedia> digitalMedias, bool raiseEvents = true)
        {
            var digitalMediaArray = digitalMedias as IDigitalMedia[] ?? digitalMedias.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMediaArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    foreach (var digitalMedia in digitalMediaArray)
                    {
                        repository.Delete(digitalMedia);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMediaArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IDigitalMedia"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        public IDigitalMedia GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateDigitalMediaRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IDigitalMedia"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        public IEnumerable<IDigitalMedia> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateDigitalMediaRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Returns a collection of all <see cref="IDigitalMedia"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal IEnumerable<IDigitalMedia> GetAll()
        {
            using (var repository = _repositoryFactory.CreateDigitalMediaRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        } 
    }
}