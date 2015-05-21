namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents the OfferSettingsService.
    /// </summary>
    internal class OfferSettingsService : IOfferSettingsService
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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsService"/> class.
        /// </summary>
        public OfferSettingsService()
            : this(new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public OfferSettingsService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public OfferSettingsService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IOfferSettingsService, Events.NewEventArgs<IOfferSettings>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IOfferSettingsService, Events.NewEventArgs<IOfferSettings>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IOfferSettingsService, SaveEventArgs<IOfferSettings>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IOfferSettingsService, SaveEventArgs<IOfferSettings>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IOfferSettingsService, DeleteEventArgs<IOfferSettings>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IOfferSettingsService, DeleteEventArgs<IOfferSettings>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings CreateOfferSettings(string name, string offerCode, Guid offerProviderKey, bool raiseEvents = true)
        {
            return CreateOfferSettings(
                name,
                offerCode,
                offerProviderKey,
                new OfferComponentDefinitionCollection(),
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="componentDefinitions">
        /// The component definitions.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings CreateOfferSettings(
            string name,
            string offerCode,
            Guid offerProviderKey,
            OfferComponentDefinitionCollection componentDefinitions,
            bool raiseEvents = true)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(offerCode, "offerCode");
            Mandate.ParameterCondition(!Guid.Empty.Equals(offerProviderKey), "offerProviderKey");
            Mandate.ParameterNotNull(componentDefinitions, "componentDefinitions");

            var offerSettings = new OfferSettings(name, offerCode, offerProviderKey, componentDefinitions);

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IOfferSettings>(offerSettings), this))
            {
                offerSettings.WasCancelled = true;
            }

            return offerSettings;
        }

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> and saves it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings CreateOfferSettingsWithKey(string name, string offerCode, Guid offerProviderKey, bool raiseEvents = true)
        {
            return CreateOfferSettingsWithKey(
                name,
                offerCode,
                offerProviderKey,
                new OfferComponentDefinitionCollection(),
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IOfferSettings"/> and saves it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="componentDefinitions">
        /// The component definitions.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings CreateOfferSettingsWithKey(
            string name,
            string offerCode,
            Guid offerProviderKey,
            OfferComponentDefinitionCollection componentDefinitions,
            bool raiseEvents = true)
        {
            var offerSettings = CreateOfferSettings(name, offerCode, offerProviderKey, componentDefinitions, raiseEvents);
            if (((OfferSettings)offerSettings).WasCancelled) return offerSettings;

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOfferSettingsRepository(uow))
                {
                    repository.AddOrUpdate(offerSettings);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IOfferSettings>(offerSettings), this);

            return offerSettings;
        }

        /// <summary>
        /// Saves a single <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IOfferSettings offerSettings, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IOfferSettings>(offerSettings), this))
            {
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOfferSettingsRepository(uow))
                {
                    repository.AddOrUpdate(offerSettings);
                    uow.Commit();
                }
            }
            
            if (raiseEvents)
            Saved.RaiseEvent(new SaveEventArgs<IOfferSettings>(offerSettings), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offersSettings">
        /// The offers settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IEnumerable<IOfferSettings> offersSettings, bool raiseEvents = true)
        {
            var settingsArray = offersSettings as IOfferSettings[] ?? offersSettings.ToArray();

            if (raiseEvents)
            Saving.RaiseEvent(new SaveEventArgs<IOfferSettings>(settingsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOfferSettingsRepository(uow))
                {
                    foreach (var setting in settingsArray)
                    {
                        repository.AddOrUpdate(setting);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents)
            Saved.RaiseEvent(new SaveEventArgs<IOfferSettings>(settingsArray), this);
        }

        /// <summary>
        /// Deletes a single of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(IOfferSettings offerSettings, bool raiseEvents = true)
        {
            if (raiseEvents) 
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IOfferSettings>(offerSettings), this)) return;


            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOfferSettingsRepository(uow))
                {
                    repository.Delete(offerSettings);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<IOfferSettings>(offerSettings), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="offersSettings">
        /// The offers settings.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(IEnumerable<IOfferSettings> offersSettings, bool raiseEvents = true)
        {
            var settingsArray = offersSettings as IOfferSettings[] ?? offersSettings.ToArray();

            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IOfferSettings>(settingsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateOfferSettingsRepository(uow))
                {
                    foreach (var setting in settingsArray)
                    {
                        repository.Delete(setting);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<IOfferSettings>(settingsArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IOfferSettings"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateOfferSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IOfferSettings"/> for a given offer provider.
        /// </summary>
        /// <param name="offerProviderKey">
        /// The offer provider key.
        /// </param>
        /// <param name="activeOnly">
        /// Optional value indicating whether or not to only return active Offers settings marked as active
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings}"/>.
        /// </returns>
        public IEnumerable<IOfferSettings> GetByOfferProviderKey(Guid offerProviderKey, bool activeOnly = true)
        {
            using (var repository = _repositoryFactory.CreateOfferSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = activeOnly ? 
                    Query<IOfferSettings>.Builder.Where(x => x.OfferProviderKey == offerProviderKey && x.Active) :
                    Query<IOfferSettings>.Builder.Where(x => x.OfferProviderKey == offerProviderKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a <see cref="OfferSettings"/> by the offer code value.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        public IOfferSettings GetByOfferCode(string offerCode)
        {
            using (var repository = _repositoryFactory.CreateOfferSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IOfferSettings>.Builder.Where(x => x.OfferCode == offerCode);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection of active <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="excludeExpired">
        /// The exclude Expired.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings"/>.
        /// </returns>
        public IEnumerable<IOfferSettings> GetAllActive(bool excludeExpired = true)
        {
            using (var repository = _repositoryFactory.CreateOfferSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = excludeExpired
                                ? Query<IOfferSettings>.Builder.Where(x => x.Active && x.OfferEndsDate <= DateTime.Now)
                                : Query<IOfferSettings>.Builder.Where(x => x.Active);

                return repository.GetByQuery(query);
            }
        }
    }
}