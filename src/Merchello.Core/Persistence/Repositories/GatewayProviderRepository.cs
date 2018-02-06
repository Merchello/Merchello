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
    using Merchello.Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The gateway provider repository.
    /// </summary>
    internal class GatewayProviderRepository : MerchelloPetaPocoRepositoryBase<IGatewayProviderSettings>, IGatewayProviderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public GatewayProviderRepository(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax) 
            : base(work, logger, sqlSyntax)
        { 
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProviderSettings"/> by ship country key.
        /// </summary>
        /// <param name="shipCountryKey">
        /// The ship country key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IGatewayProviderSettings}"/>.
        /// </returns>
        public IEnumerable<IGatewayProviderSettings> GetGatewayProvidersByShipCountryKey(Guid shipCountryKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ShipMethodDto>(SqlSyntax)
                .InnerJoin<GatewayProviderSettingsDto>(SqlSyntax)
                .On<ShipMethodDto, GatewayProviderSettingsDto>(SqlSyntax, left => left.ProviderKey, right => right.Key)
                .Where<ShipMethodDto>(x => x.ShipCountryKey == shipCountryKey);

            var dtos = Database.Fetch<ShipMethodDto, GatewayProviderSettingsDto>(sql);
            var factory = new GatewayProviderSettingsFactory();
            return dtos.DistinctBy(x => x.GatewayProviderSettingsDto.Key).Select(dto => factory.BuildEntity(dto.GatewayProviderSettingsDto));
        }

        /// <summary>
        /// Gets <see cref="IGatewayProviderSettings"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IGatewayProviderSettings"/>.
        /// </returns>
        protected override IGatewayProviderSettings PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new {Key = key});

            var dto = Database.Fetch<GatewayProviderSettingsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new GatewayProviderSettingsFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets all <see cref="GatewayProviderSettings"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IGatewayProviderSettings}"/>.
        /// </returns>
        protected override IEnumerable<IGatewayProviderSettings> PerformGetAll(params Guid[] keys)
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
                var factory = new GatewayProviderSettingsFactory();
                var dtos = Database.Fetch<GatewayProviderSettingsDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IGatewayProviderSettings"/> by a query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IGatewayProviderSettings}"/>.
        /// </returns>
        protected override IEnumerable<IGatewayProviderSettings> PerformGetByQuery(IQuery<IGatewayProviderSettings> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IGatewayProviderSettings>(sqlClause, query);

            var sql = translator.Translate();

            var dtos = Database.Fetch<GatewayProviderSettingsDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL.
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
                .From<GatewayProviderSettingsDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchGatewayProviderSettings.pk = @Key";
        }

        /// <summary>
        /// Gets the list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {            
            var list = new List<string>
            {                
                "DELETE FROM merchGatewayProviderSettings WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IGatewayProviderSettings entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new GatewayProviderSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IGatewayProviderSettings entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new GatewayProviderSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            
            entity.ResetDirtyProperties();
        }
    }
}