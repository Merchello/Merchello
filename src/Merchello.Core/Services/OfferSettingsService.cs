namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using umbraco.BusinessLogic;
    using umbraco.cms.presentation;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    using RepositoryFactory = Merchello.Core.Persistence.RepositoryFactory;

    /// <summary>
    /// Represents the OfferSettingsService.
    /// </summary>
    internal class OfferSettingsService : MerchelloRepositoryService, IOfferSettingsService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsService"/> class.
        /// </summary>
        public OfferSettingsService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public OfferSettingsService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public OfferSettingsService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
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
        /// <param name="logger">
        /// The logger.
        /// </param>
        public OfferSettingsService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
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
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        public OfferSettingsService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
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
            Ensure.ParameterNotNullOrEmpty(name, "name");
            Ensure.ParameterNotNullOrEmpty(offerCode, "offerCode");
            Ensure.ParameterCondition(!Guid.Empty.Equals(offerProviderKey), "offerProviderKey");
            Ensure.ParameterNotNull(componentDefinitions, "componentDefinitions");

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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateOfferSettingsRepository(uow))
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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateOfferSettingsRepository(uow))
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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateOfferSettingsRepository(uow))
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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateOfferSettingsRepository(uow))
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
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateOfferSettingsRepository(uow))
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
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IOfferSettings"/> by their unique keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings}"/>.
        /// </returns>
        public IEnumerable<IOfferSettings> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Returns a page of <see cref="IOfferSettings"/>.
        /// </summary>
        /// <param name="page">
        /// The page number.
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
        /// The <see cref="Page{IOfferSettings}"/>.
        /// </returns>
        public Page<IOfferSettings> GetPage(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
            {
               var query = Query<IOfferSettings>.Builder.Where(x => x.Key != Guid.Empty);

               return repository.GetPage(page, itemsPerPage, query, ValidateSortByField(sortBy), sortDirection);
            }
        }


        public Page<IOfferSettings> GetPage(
            string filterTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending,
            bool activeOnly = true)
        {
            throw new NotImplementedException();
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
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
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
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
            {
                var query = excludeExpired
                                ? Query<IOfferSettings>.Builder.Where(x => x.Active && x.OfferEndsDate <= DateTime.Now)
                                : Query<IOfferSettings>.Builder.Where(x => x.Active);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Checks if the offer code is unique.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// A valid indicating whether or not the offer code is unique.
        /// </returns>
        public bool OfferCodeIsUnique(string offerCode)
        {
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IOfferSettings>.Builder.Where(x => x.OfferCode == offerCode);
                var result = repository.GetByQuery(query);
                return !result.Any();
            }
        }

        /// <summary>
        /// Searches the offer settings by a term.
        /// </summary>
        /// <param name="filterTerm">
        /// The term.
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
        /// The <see cref="Page{IOfferSettings}"/>.
        /// </returns>
        public Page<IOfferSettings> GetPage(
            string filterTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            using (var repository = RepositoryFactory.CreateOfferSettingsRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Search(filterTerm, page, itemsPerPage, this.ValidateSortByField(sortBy), sortDirection);
            }
        }

        /// <summary>
        /// Validates the sort field.
        /// </summary>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ValidateSortByField(string sortBy)
        {
            var valid = new[] { "name", "offerCode", "offerStartsDate", "offerEndsDate", "createDate" };
            return !valid.Contains(sortBy, StringComparer.CurrentCultureIgnoreCase) ? "name" : sortBy;
        }
    }
}