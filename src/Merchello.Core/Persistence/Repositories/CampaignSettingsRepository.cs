namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;
    
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    /// <summary>
    /// Represents a CampaignSettingsRepository.
    /// </summary>
    internal class CampaignSettingsRepository : MerchelloPetaPocoRepositoryBase<ICampaignSettings>, ICampaignSettingsRepository
    {
        /// <summary>
        /// The <see cref="ICampaignActivitySettingsRepository"/>.
        /// </summary>
        private readonly ICampaignActivitySettingsRepository _campaignActivitySettingsRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="campaignActivitySettingsRepository">
        /// The <see cref="ICampaignActivitySettingsRepository"/>.
        /// </param>
        public CampaignSettingsRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ICampaignActivitySettingsRepository campaignActivitySettingsRepository)
            : base(work, cache)
        {
            Mandate.ParameterNotNull(campaignActivitySettingsRepository, "campaignActivitySettingsRepository");

            _campaignActivitySettingsRepository = campaignActivitySettingsRepository;
        }

        /// <summary>
        /// The perform get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignSettings"/>.
        /// </returns>
        protected override ICampaignSettings PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<CampaignSettingsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var activitySettings = _campaignActivitySettingsRepository.GetByCampaignKey(key).OrderBy(x => x.StartDate);

            var factory = new CampaignSettingsFactory(activitySettings);

            var settings = factory.BuildEntity(dto);

            return settings;
        }

        /// <summary>
        /// The perform get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignSettings}"/>.
        /// </returns>
        protected override IEnumerable<ICampaignSettings> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {
                var dtos = Database.Fetch<CampaignSettingsDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        /// <summary>
        /// The perform get by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignSettings}"/>.
        /// </returns>
        protected override IEnumerable<ICampaignSettings> PerformGetByQuery(IQuery<ICampaignSettings> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICampaignSettings>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CampaignSettingsDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// The get base query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchCampaignSettings");

            return sql;
        }

        /// <summary>
        /// The get base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchCampaignSettings.pk = @Key";
        }

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of SQL delete clauses.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchCampaignActivitySettings WHERE campaignKey = @Key",
                    "DELETE FROM merchCampaignSettings WHERE pk = @Key",
                };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(ICampaignSettings entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new CampaignSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(ICampaignSettings entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CampaignSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}