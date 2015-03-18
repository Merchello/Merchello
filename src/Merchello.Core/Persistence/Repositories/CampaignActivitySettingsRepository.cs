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
    /// Represents a CampaignActivitySettingsRepository.
    /// </summary>
    internal class CampaignActivitySettingsRepository : MerchelloPetaPocoRepositoryBase<ICampaignActivitySettings>, ICampaignActivitySettingsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivitySettingsRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public CampaignActivitySettingsRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        /// <summary>
        /// Returns a collection of <see cref="ICampaignActivitySettings"/> for a given campaign.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        public IEnumerable<ICampaignActivitySettings> GetByCampaignKey(Guid campaignKey)
        {
            var query = Querying.Query<ICampaignActivitySettings>.Builder.Where(x => x.CampaignKey == campaignKey);
            return this.GetByQuery(query);
        }

        /// <summary>
        /// The perform get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        protected override ICampaignActivitySettings PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<CampaignActivitySettingsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CampaignActivitySettingsFactory();

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
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        protected override IEnumerable<ICampaignActivitySettings> PerformGetAll(params Guid[] keys)
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
                var factory = new CampaignActivitySettingsFactory();
                var dtos = Database.Fetch<CampaignActivitySettingsDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
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
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        protected override IEnumerable<ICampaignActivitySettings> PerformGetByQuery(IQuery<ICampaignActivitySettings> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICampaignActivitySettings>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CampaignActivitySettingsDto>(sql);

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
               .From("merchCampaignActivitySettings");

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
            return "merchCampaignActivitySettings.pk = @Key";
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
                    "DELETE FROM merchCampaignActivitySettings WHERE pk = @Key",
                };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(ICampaignActivitySettings entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new CampaignActivitySettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<ICampaignSettings>(entity.CampaignKey));
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(ICampaignActivitySettings entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CampaignActivitySettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<ICampaignSettings>(entity.CampaignKey));
        }

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(ICampaignActivitySettings entity)
        {
            base.PersistDeletedItem(entity);

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<ICampaignSettings>(entity.CampaignKey));
        }
    }
}