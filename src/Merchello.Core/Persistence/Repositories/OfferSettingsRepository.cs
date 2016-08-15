namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using umbraco;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents an offer settings repository.
    /// </summary>
    internal class OfferSettingsRepository : PagedRepositoryBase<IOfferSettings, OfferSettingsDto>, IOfferSettingsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsRepository"/> class.
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
        public OfferSettingsRepository(IDatabaseUnitOfWork work, CacheHelper cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
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
        /// Searches the offer settings by a term.
        /// </summary>
        /// <param name="term">
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
        public Page<IOfferSettings> Search(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = this.BuildOfferSearchSql(term);
            var p = this.GetDtoPage(page, itemsPerPage, sql, sortBy, sortDirection);
            return this.MapPageDtoToPageEntity(p);
        }

        /// <summary>
        /// Performs the get by key operation.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferSettings"/>.
        /// </returns>
        protected override IOfferSettings PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OfferSettingsDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new OfferSettingsFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Performs the get all operation.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferSettings}"/>.
        /// </returns>
        protected override IEnumerable<IOfferSettings> PerformGetAll(params Guid[] keys)
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
                var factory = new OfferSettingsFactory();
                var dtos = Database.Fetch<OfferSettingsDto>(GetBaseQuery(false));
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
        /// The <see cref="IEnumerable{IOfferSettings}"/>.
        /// </returns>
        protected override IEnumerable<IOfferSettings> PerformGetByQuery(IQuery<IOfferSettings> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IOfferSettings>(sqlClause, query);

            var sql = translator.Translate();

            var dtos = Database.Fetch<OfferSettingsDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Constructs a base SQL query
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
                .From<OfferSettingsDto>(SqlSyntax);

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
            return "merchOfferSettings.pk = @Key";
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
                "UPDATE merchOfferRedeemed SET offerSettingsKey = NULL WHERE offerSettingsKey = @Key",
                "DELETE FROM merchOfferSettings WHERE pk = @Key"                        
            };

            return list;
        }

        /// <summary>
        /// Adds a new offer settings entity to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IOfferSettings entity)
        {
            ((Entity)entity).AddingEntity();
            var factory = new OfferSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;
            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an offer settings entity in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IOfferSettings entity)
        {
            ((Entity)entity).AddingEntity();
            var factory = new OfferSettingsFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
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
            sql.Select("*").From<OfferSettingsDto>(SqlSyntax);
            if (terms.Any())
            {
                sql.Where("name LIKE @term OR offerCode LIKE @offerCode", new { @term = string.Format("%{0}%", string.Join("% ", terms)).Trim(), offerCode = string.Format("%{0}%", string.Join("% ", terms)).Trim() });
            }
            else
            {
                sql.Where("name LIKE @term OR offerCode LIKE @term", new { @term = string.Format("%{0}%", string.Join("% ", terms)).Trim() });
            }

            return sql;
        }
    }
}