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
    /// The invoice line item repository.
    /// </summary>
    internal class InvoiceLineItemRepository : LineItemRepositoryBase<IInvoiceLineItem>, IInvoiceLineItemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceLineItemRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The <see cref="IDatabaseUnitOfWork"/>.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public InvoiceLineItemRepository(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Gets a <see cref="IInvoiceLineItem"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IInvoiceLineItem"/>.
        /// </returns>
        protected override IInvoiceLineItem PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
             .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<InvoiceItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new InvoiceLineItemFactory();
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets a collection of all <see cref="IInvoiceLineItem"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IInvoiceLineItem}"/>.
        /// </returns>
        protected override IEnumerable<IInvoiceLineItem> PerformGetAll(params Guid[] keys)
        {

            var dtos = new List<InvoiceItemDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<InvoiceItemDto>(GetBaseQuery(false).WhereIn<InvoiceItemDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<InvoiceItemDto>(GetBaseQuery(false));
            }

            var factory = new InvoiceLineItemFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IInvoiceLineItem"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IInvoiceLineItem}"/>.
        /// </returns>
        protected override IEnumerable<IInvoiceLineItem> PerformGetByQuery(IQuery<IInvoiceLineItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoiceLineItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// The base SQL query.
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
                .From<InvoiceItemDto>(SqlSyntax);

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
            return "merchInvoiceItem.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IInvoiceLineItem}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchInvoiceItem WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IInvoiceLineItem entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new InvoiceLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IInvoiceLineItem entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new InvoiceLineItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}