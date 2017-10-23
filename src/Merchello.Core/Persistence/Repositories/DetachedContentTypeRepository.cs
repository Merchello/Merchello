namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
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
    /// Represents a DetachedContentTypeRepository.
    /// </summary>
    internal class DetachedContentTypeRepository : MerchelloPetaPocoRepositoryBase<IDetachedContentType>, IDetachedContentTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeRepository"/> class.
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
        /// The SQL Syntax.
        /// </param>
        public DetachedContentTypeRepository(IDatabaseUnitOfWork work, CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Gets a <see cref="IDetachedContentType"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        protected override IDetachedContentType PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<DetachedContentTypeDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new DetachedContentTypeFactory();

            var payment = factory.BuildEntity(dto);

            return payment;
        }

        /// <summary>
        /// Performs get all <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{DetachedContentType}"/>.
        /// </returns>
        protected override IEnumerable<IDetachedContentType> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var id in keys)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new DetachedContentTypeFactory();
                var dtos = Database.Fetch<DetachedContentTypeDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IDetachedContentType"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        protected override IEnumerable<IDetachedContentType> PerformGetByQuery(IQuery<IDetachedContentType> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IDetachedContentType>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<DetachedContentTypeDto>(sql);

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
               .From<DetachedContentTypeDto>(SqlSyntax);

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
            return "merchDetachedContentType.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses to be executed.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "UPDATE merchProductOption SET detachedContentTypeKey = NULL WHERE detachedContentTypeKey = @Key",
                    "DELETE FROM merchProductVariantDetachedContent WHERE merchProductVariantDetachedContent.detachedContentTypeKey = @Key",
                    "DELETE FROM merchDetachedContentType WHERE merchDetachedContentType.pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// Inserts a new <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IDetachedContentType entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new DetachedContentTypeFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IDetachedContentType entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new DetachedContentTypeFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IDetachedContentType>(entity.Key));
        }
    }
}