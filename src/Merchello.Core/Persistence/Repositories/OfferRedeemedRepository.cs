namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Migrations.Initial;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The offer redeemed repository.
    /// </summary>
    internal class OfferRedeemedRepository : PagedRepositoryBase<IOfferRedeemed, OfferRedeemedDto>, IOfferRedeemedRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRedeemedRepository"/> class.
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
        public OfferRedeemedRepository(IDatabaseUnitOfWork work, CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// Searches for a set of keys that match the term submitted.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = this.BuildOfferSearchSql(searchTerm);

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Performs the get by key operation.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedeemed"/>.
        /// </returns>
        protected override IOfferRedeemed PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OfferRedeemedDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new OfferRedeemedFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Performs the get all operation.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        protected override IEnumerable<IOfferRedeemed> PerformGetAll(params Guid[] keys)
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
                var factory = new OfferRedeemedFactory();
                var dtos = Database.Fetch<OfferRedeemedDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Performs the get by query operation.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferRedeemed}"/>.
        /// </returns>
        protected override IEnumerable<IOfferRedeemed> PerformGetByQuery(IQuery<IOfferRedeemed> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IOfferRedeemed>(sqlClause, query);

            var sql = translator.Translate();

            var dtos = Database.Fetch<OfferRedeemedDto>(sql);

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
                .From<OfferRedeemedDto>(SqlSyntax);

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
            return "merchOfferRedeemed.pk = @Key";
        }

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchOfferRedeemed WHERE pk = @Key"                        
            };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IOfferRedeemed entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new OfferRedeemedFactory();
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
        protected override void PersistUpdatedItem(IOfferRedeemed entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new OfferRedeemedFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();

            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IOfferRedeemed>(entity.Key));
            RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IItemCache>(entity.Key));
        }

        /// <summary>
        /// Builds an offer search query.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        private Sql BuildOfferSearchSql(string searchTerm)
        {
            searchTerm = searchTerm.Replace(",", " ");
            var terms = searchTerm.Split(' ').Select(x => x.Trim()).ToArray();

            var sql = new Sql();
            sql.Select("*").From<OfferRedeemedDto>(SqlSyntax);

            sql.Where("offerCode LIKE @term", new { @term = string.Format("%{0}%", string.Join("% ", terms)).Trim() });

            return sql;
        }
    }
}