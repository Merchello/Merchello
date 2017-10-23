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
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The tax method repository.
    /// </summary>
    internal class TaxMethodRepository : MerchelloPetaPocoRepositoryBase<ITaxMethod>, ITaxMethodRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public TaxMethodRepository(
            IDatabaseUnitOfWork work,
            CacheHelper cache,
            ILogger logger,
            ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethod"/>.
        /// </returns>
        protected override ITaxMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<TaxMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new TaxMethodFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets the collection of all <see cref="ITaxMethod"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ITaxMethod}"/>.
        /// </returns>
        protected override IEnumerable<ITaxMethod> PerformGetAll(params Guid[] keys)
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
                var factory = new TaxMethodFactory();
                var dtos = Database.Fetch<TaxMethodDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ITaxMethod}"/>.
        /// </returns>
        protected override IEnumerable<ITaxMethod> PerformGetByQuery(IQuery<ITaxMethod> query)
        {

            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ITaxMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<TaxMethodDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL query.
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
                .From<TaxMethodDto>(SqlSyntax);

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
            return "merchTaxMethod.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchTaxMethod WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(ITaxMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new TaxMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Saves an existing item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(ITaxMethod entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new TaxMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Core.Cache.CacheKeys.GetEntityCacheKey<ITaxMethod>(entity.Key));
        }
    }
}