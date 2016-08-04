namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using umbraco.cms.presentation;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents an EntityCollectionRepository.
    /// </summary>
    internal class EntityCollectionRepository : MerchelloPetaPocoRepositoryBase<IEntityCollection>, IEntityCollectionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionRepository"/> class.
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
        public EntityCollectionRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
        }

        /// <summary>
        /// The get entity collections by product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        public IEnumerable<IEntityCollection> GetEntityCollectionsByProductKey(Guid productKey)
        {
            var sql =
                this.GetBaseQuery(false)
                    .Append("WHERE [merchEntityCollection].[pk] IN (")
                    .Append("SELECT DISTINCT([entityCollectionKey])")
                    .Append("FROM [merchProduct2EntityCollection]")
                    .Append("WHERE [merchProduct2EntityCollection].[productKey] = @pkey", new { @pkey = productKey })
                    .Append(")");
            var dtos = Database.Fetch<EntityCollectionDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(x => Get(x.Key));
        }

        /// <summary>
        /// The get entity collections by invoice key.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        public IEnumerable<IEntityCollection> GetEntityCollectionsByInvoiceKey(Guid invoiceKey)
        {
            var sql =
            this.GetBaseQuery(false)
                .Append("WHERE [merchEntityCollection].[pk] IN (")
                .Append("SELECT DISTINCT([entityCollectionKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append("WHERE [merchInvoice2EntityCollection].[invoiceKey] = @ikey", new { @ikey = invoiceKey })
                .Append(")");


            var dtos = Database.Fetch<EntityCollectionDto>(sql);
            return dtos.DistinctBy(x => x.Key).Select(x => Get(x.Key));
        }

        /// <summary>
        /// The get entity collections by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        public IEnumerable<IEntityCollection> GetEntityCollectionsByCustomerKey(Guid customerKey)
        {
            var sql =
            this.GetBaseQuery(false)
                .Append("WHERE [merchEntityCollection].[pk] IN (")
                .Append("SELECT DISTINCT([entityCollectionKey])")
                .Append("FROM [merchCustomer2EntityCollection]")
                .Append("WHERE [merchCustomer2EntityCollection].[customerKey] = @ckey", new { @ckey = customerKey })
                .Append(")");

            var dtos = Database.Fetch<EntityCollectionDto>(sql);
            return dtos.DistinctBy(x => x.Key).Select(x => Get(x.Key));
            //var factory = new EntityCollectionFactory();

            //return dtos.Select(factory.BuildEntity);
        }

        /// <summary>
        /// The get page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IEntityCollection}"/>.
        /// </returns>
        public Page<IEntityCollection> GetPage(
            long page,
            long itemsPerPage,
            IQuery<IEntityCollection> query,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sqlClause = new Sql();
            sqlClause.Select("*").From<EntityCollectionDto>(SqlSyntax);

            var translator = new SqlTranslator<IEntityCollection>(sqlClause, query);
            var sql = translator.Translate();

            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            var dtoPage = Database.Page<EntityCollectionDto>(page, itemsPerPage, sql);

            return new Page<IEntityCollection>()
            {
                CurrentPage = dtoPage.CurrentPage,
                ItemsPerPage = dtoPage.ItemsPerPage,
                TotalItems = dtoPage.TotalItems,
                TotalPages = dtoPage.TotalPages,
                Items = dtoPage.Items.Select(x => Get(x.Key)).ToList()
            };
        }

        /// <summary>
        /// The perform get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        protected override IEntityCollection PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<EntityCollectionDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new EntityCollectionFactory();

            var collection = factory.BuildEntity(dto);

            return collection;
        }

        /// <summary>
        /// The perform get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection"/>.
        /// </returns>
        protected override IEnumerable<IEntityCollection> PerformGetAll(params Guid[] keys)
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
                var dtos = Database.Fetch<EntityCollectionDto>(GetBaseQuery(false));
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
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        protected override IEnumerable<IEntityCollection> PerformGetByQuery(IQuery<IEntityCollection> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IEntityCollection>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<EntityCollectionDto>(sql);

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
               .From<EntityCollectionDto>(SqlSyntax);

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
            return "merchEntityCollection.pk = @Key";
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
                    "DELETE FROM merchProduct2EntityCollection WHERE merchProduct2EntityCollection.entityCollectionKey = @Key",
                    "DELETE FROM merchInvoice2EntityCollection WHERE merchInvoice2EntityCollection.entityCollectionKey = @Key",
                    "DELETE FROM merchCustomer2EntityCollection WHERE merchCustomer2EntityCollection.entityCollectionKey = @Key",
                    "DELETE FROM merchEntityCollection WHERE merchEntityCollection.pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IEntityCollection entity)
        {
            var query = entity.ParentKey == null
                            ? Querying.Query<IEntityCollection>.Builder.Where(x => x.ProviderKey == entity.ProviderKey && x.EntityTfKey == entity.EntityTfKey)
                            : Querying.Query<IEntityCollection>.Builder.Where(x => x.ParentKey == entity.ParentKey);

            var sortOrder = this.Count(query);
            ((EntityCollection)entity).SortOrder = sortOrder;

            ((Entity)entity).AddingEntity();

            var factory = new EntityCollectionFactory();
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
        protected override void PersistUpdatedItem(IEntityCollection entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new EntityCollectionFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}