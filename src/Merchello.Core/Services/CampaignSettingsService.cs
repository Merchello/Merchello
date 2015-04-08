namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Lucene.Net.Search;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a CampaignSettingsSerivce.
    /// </summary>
    public class CampaignSettingsService : ICampaignSettingsService
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
        /// The <see cref="ICampaignActivitySettingsService"/>.
        /// </summary>
        private readonly ICampaignActivitySettingsService _campaignActivitySettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsService"/> class.
        /// </summary>
        public CampaignSettingsService()
            : this(new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public CampaignSettingsService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, new CampaignActivitySettingsService())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="campaignActivitySettingsService">
        /// The <see cref="ICampaignActivitySettingsService"/>.
        /// </param>
        public CampaignSettingsService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ICampaignActivitySettingsService campaignActivitySettingsService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(campaignActivitySettingsService, "campaignActivitySettingsService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _campaignActivitySettingsService = campaignActivitySettingsService;
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICampaignSettingsService, Events.NewEventArgs<ICampaignSettings>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICampaignSettingsService, Events.NewEventArgs<ICampaignSettings>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICampaignSettingsService, SaveEventArgs<ICampaignSettings>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICampaignSettingsService, SaveEventArgs<ICampaignSettings>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICampaignSettingsService, DeleteEventArgs<ICampaignSettings>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICampaignSettingsService, DeleteEventArgs<ICampaignSettings>> Deleted;

        #endregion


        /// <summary>
        /// Creates a <see cref="ICampaignSettings"/> without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignSettings"/>.
        /// </returns>
        public ICampaignSettings CreateCampaignSettings(string name, string alias, bool raiseEvents = true)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            var campaign = new CampaignSettings()
                {
                    Name = name,
                    Alias = alias,
                    Active = true,
                    ActivitySettings = Enumerable.Empty<ICampaignActivitySettings>()
                };

            if (raiseEvents)
            if (!Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICampaignSettings>(campaign), this))
            {
                return campaign;
            }

            campaign.WasCancelled = true;

            return campaign;
        }

        /// <summary>
        /// Creates a <see cref="ICampaignSettings"/> and saves it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignSettings"/>.
        /// </returns>
        public ICampaignSettings CreateCampaignSettingsWithKey(string name, string alias, bool raiseEvents = true)
        {
            var campaign = this.CreateCampaignSettings(name, alias, raiseEvents);

            if (((CampaignSettings)campaign).WasCancelled) return campaign;

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(uow))
                {
                    repository.AddOrUpdate(campaign);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<ICampaignSettings>(campaign), this);

            return campaign;
        }

        /// <summary>
        /// Saves a single <see cref="ICampaignSettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Save(ICampaignSettings settings, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICampaignSettings>(settings), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(uow))
                {
                    repository.AddOrUpdate(settings);
                    uow.Commit();
                }
            }


            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICampaignSettings>(settings), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="ICampaignSettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Save(IEnumerable<ICampaignSettings> settings, bool raiseEvents = true)
        {
            var settingsArray = settings as ICampaignSettings[] ?? settings.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICampaignSettings>(settingsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();

                using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(uow))
                {
                    foreach (var setting in settingsArray)
                    {
                        repository.AddOrUpdate(setting);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICampaignSettings>(settingsArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICampaignSettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(ICampaignSettings settings, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICampaignSettings>(settings), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(uow))
                {
                    repository.Delete(settings);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICampaignSettings>(settings), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ICampaignSettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<ICampaignSettings> settings, bool raiseEvents = true)
        {
            var settingsArray = settings as ICampaignSettings[] ?? settings.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICampaignSettings>(settingsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(uow))
                {
                    foreach (var setting in settingsArray)
                    {
                        repository.Delete(setting);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICampaignSettings>(settingsArray), this);
        }

        /// <summary>
        /// Gets a <see cref="ICampaignSettings"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignSettings"/>.
        /// </returns>
        public ICampaignSettings GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICampaignSettings"/> by a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignSettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignSettings> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets the collection of all <see cref="ICampaignSettings"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignSettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignSettings> GetAll()
        {
            using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets the collection of all "active" <see cref="ICampaignSettings"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignSetting}"/>.
        /// </returns>
        public IEnumerable<ICampaignSettings> GetActive()
        {
            using (var repository = _repositoryFactory.CreateCampaignSettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICampaignSettings>.Builder.Where(x => x.Active);
                return repository.GetByQuery(query);
            }
        }

        #region Campain Activity

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> without saving it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="campaignActivityType">
        /// The campaign activity type.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        /// <remarks>
        /// This cannot be used with Custom types!
        /// </remarks>
        public ICampaignActivitySettings CreateCampaignActivitySettings(
            Guid campaignKey,
            string name,
            string alias,
            CampaignActivityType campaignActivityType,
            DateTime startDate,
            DateTime endDate,
            bool raiseEvents = true)
        {
            return _campaignActivitySettingsService.CreateCampaignActivitySettings(
                campaignKey,
                name,
                alias,
                campaignActivityType,
                startDate,
                endDate,
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> without saving it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="campaignActivityTfKey">
        /// The campaign activity type field key.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        public ICampaignActivitySettings CreateCampaignActivitySettings(
            Guid campaignKey,
            string name,
            string alias,
            Guid campaignActivityTfKey,
            DateTime startDate,
            DateTime endDate,
            bool raiseEvents = true)
        {
            return _campaignActivitySettingsService.CreateCampaignActivitySettings(
                campaignKey,
                name,
                alias,
                campaignActivityTfKey,
                startDate,
                endDate,
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> and saves it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="campaignActivityTfKey">
        /// The campaign activity type field key.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        public ICampaignActivitySettings CreateCampaignActivitySettingsWithKey(
            Guid campaignKey,
            string name,
            string alias,
            Guid campaignActivityTfKey,
            DateTime startDate,
            DateTime endDate,
            bool raiseEvents = true)
        {
            return _campaignActivitySettingsService.CreateCampaignActivitySettingsWithKey(
                campaignKey,
                name,
                alias,
                campaignActivityTfKey,
                startDate,
                endDate,
                raiseEvents);
        }

        /// <summary>
        /// Saves a single <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Save(ICampaignActivitySettings settings, bool raiseEvents = true)
        {
            _campaignActivitySettingsService.Save(settings, raiseEvents);
        }

        /// <summary>
        /// Deletes a single <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(ICampaignActivitySettings settings, bool raiseEvents = true)
        {
            _campaignActivitySettingsService.Delete(settings, raiseEvents);
        }

        /// <summary>
        /// Gets a <see cref="ICampaignActivitySettings"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        public ICampaignActivitySettings GetCampaignActivitySettingsByKey(Guid key)
        {
            return _campaignActivitySettingsService.GetByKey(key);
        }

        /// <summary>
        /// Gets a collection of all CampaignActivities
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignActivitySettings> GetAllCampaignActivitySettings()
        {
            return _campaignActivitySettingsService.GetAll();
        }

        /// <summary>
        /// Gets a collection of <see cref="ICampaignActivitySettings"/> for a given campaign.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignActivitySettings> GetByCampaignKey(Guid campaignKey)
        {
            return _campaignActivitySettingsService.GetByCampaignKey(campaignKey);
        }

        /// <summary>
        /// Gets a collection of "active" <see cref="ICampaignActivitySettings"/> for a given campaign
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignActivitySettings> GetActiveByCampaignKey(Guid campaignKey)
        {
            return _campaignActivitySettingsService.GetActiveByCampaignKey(campaignKey);
        }

        #endregion
    }
}