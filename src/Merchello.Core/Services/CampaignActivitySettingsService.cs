namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Lucene.Net.Search;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a CampaignActivitySettingsService.
    /// </summary>
    internal class CampaignActivitySettingsService : ICampaignActivitySettingsService
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
        /// Initializes a new instance of the <see cref="CampaignActivitySettingsService"/> class.
        /// </summary>
        public CampaignActivitySettingsService()
            : this(new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivitySettingsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public CampaignActivitySettingsService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivitySettingsService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        public CampaignActivitySettingsService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
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
        public static event TypedEventHandler<ICampaignActivitySettingsService, Events.NewEventArgs<ICampaignActivitySettings>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICampaignActivitySettingsService, Events.NewEventArgs<ICampaignActivitySettings>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICampaignActivitySettingsService, SaveEventArgs<ICampaignActivitySettings>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICampaignActivitySettingsService, SaveEventArgs<ICampaignActivitySettings>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICampaignActivitySettingsService, DeleteEventArgs<ICampaignActivitySettings>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICampaignActivitySettingsService, DeleteEventArgs<ICampaignActivitySettings>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> without saving it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key
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
            Mandate.ParameterCondition(!campaignActivityType.Equals(CampaignActivityType.Custom), "campaignActivityType");

            return CreateCampaignActivitySettings(
                campaignKey,
                name,
                alias,
                EnumTypeFieldConverter.CampaignActivity.GetTypeField(campaignActivityType).TypeKey,
                startDate,
                endDate,
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> without saving it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key
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
            Mandate.ParameterCondition(!campaignKey.Equals(Guid.Empty), "campaignKey");
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(alias, "alias");

            var activity = new CampaignActivitySettings(campaignKey, campaignActivityTfKey)
            {
                Name = name,
                Alias = alias,
                StartDate = startDate,
                EndDate = endDate,
                Description = string.Empty,
                ExtendedData = new ExtendedDataCollection(),
                Active = true
            };

            if (raiseEvents)
                if (!Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICampaignActivitySettings>(activity), this))
                {
                    return activity;
                }

            activity.WasCancelled = true;

            return activity;
        }

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> and saves it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key
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
            var activity = CreateCampaignActivitySettings(
                campaignKey,
                name,
                alias,
                campaignActivityTfKey,
                startDate,
                endDate,
                raiseEvents);

            if (((CampaignActivitySettings)activity).WasCancelled) return activity;

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(uow))
                {
                    repository.AddOrUpdate(activity);
                    uow.Commit();
                }
            }

            Created.RaiseEvent(new Events.NewEventArgs<ICampaignActivitySettings>(activity), this);

            return activity;
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
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICampaignActivitySettings>(settings), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(uow))
                {
                    repository.AddOrUpdate(settings);
                    uow.Commit();
                }
            }


            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICampaignActivitySettings>(settings), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Save(IEnumerable<ICampaignActivitySettings> settings, bool raiseEvents = true)
        {
            var settingsArray = settings as ICampaignActivitySettings[] ?? settings.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICampaignActivitySettings>(settingsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();

                using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(uow))
                {
                    foreach (var setting in settingsArray)
                    {
                        repository.AddOrUpdate(setting);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICampaignActivitySettings>(settingsArray), this);
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
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICampaignActivitySettings>(settings), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(uow))
                {
                    repository.Delete(settings);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICampaignActivitySettings>(settings), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<ICampaignActivitySettings> settings, bool raiseEvents = true)
        {
            var settingsArray = settings as ICampaignActivitySettings[] ?? settings.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<ICampaignActivitySettings>(settingsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(uow))
                {
                    foreach (var setting in settingsArray)
                    {
                        repository.Delete(setting);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ICampaignActivitySettings>(settingsArray), this);
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
        public ICampaignActivitySettings GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ICampaignActivitySettings"/> by a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignActivitySettings> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateCampaignActivitySettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets a collection of all CampaignActivities
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignActivitySettings> GetAll()
        {
            using (
                var repository =
                    _repositoryFactory.CreateCampaignActivitySettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
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
            using (
                var repository =
                    _repositoryFactory.CreateCampaignActivitySettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetByCampaignKey(campaignKey);
            }
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
            using (
                var repository =
                    _repositoryFactory.CreateCampaignActivitySettingsRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<ICampaignActivitySettings>.Builder.Where(
                    x => x.CampaignKey == campaignKey && x.Active);

                return repository.GetByQuery(query);
            }
        }
    }
}