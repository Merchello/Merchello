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
    /// Represents the Invoice Repository
    /// </summary>
    internal class InvoiceRepository : PagedRepositoryBase<IInvoice, InvoiceDto>, IInvoiceRepository
    {
        /// <summary>
        /// The invoice line item repository.
        /// </summary>
        private readonly IInvoiceLineItemRepository _invoiceLineItemRepository;

        /// <summary>
        /// The order repository.
        /// </summary>
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        /// The note repository.
        /// </summary>
        private readonly INoteRepository _noteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="invoiceLineItemRepository">
        /// The invoice line item repository.
        /// </param>
        /// <param name="orderRepository">
        /// The order repository.
        /// </param>
        /// <param name="noteRepository">
        /// The note Repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax
        /// </param>
        public InvoiceRepository(
            IDatabaseUnitOfWork work,
            IInvoiceLineItemRepository invoiceLineItemRepository,
            IOrderRepository orderRepository,
            INoteRepository noteRepository,
            ILogger logger,
            ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(invoiceLineItemRepository, "lineItemRepository");
            Mandate.ParameterNotNull(orderRepository, "orderRepository");
            Mandate.ParameterNotNull(noteRepository, "noteRepository");

            _invoiceLineItemRepository = invoiceLineItemRepository;
            _orderRepository = orderRepository;
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// Performs the default search by term
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
            var sql = BuildInvoiceSearchSql(searchTerm);

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Performs a search by term and a date rang
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
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
        public Page<Guid> SearchKeys(
            string searchTerm,
            DateTime startDate,
            DateTime endDate,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(searchTerm);
            sql.Where("invoiceDate BETWEEN @start AND @end", new { @start = startDate.GetDateForSqlStartOfDay(), @end = endDate.GetDateForSqlEndOfDay() });
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get max document number.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaxDocumentNumber()
        {
            var value =
                Database.ExecuteScalar<object>(
                    "SELECT TOP 1 invoiceNumber FROM merchInvoice ORDER BY invoiceNumber DESC");
            return value == null ? 0 : int.Parse(value.ToString());
        }

        #region Static Collections

        /// <summary>
        /// Returns a value indicating whether or not the invoice exists in a collection.
        /// </summary>
        /// <param name="entityKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, Guid collectionKey)
        {
            var sql = new Sql();
            sql.Append("SELECT COUNT(*)")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[invoiceKey] = @ikey AND [merchInvoice2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @ikey = entityKey, @eckey = collectionKey });

            return Database.ExecuteScalar<int>(sql) > 0;
        }


        /// <summary>
        /// Returns a value indicating whether or not the entity exists in at least one of the collections.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, Guid[] collectionKeys)
        {
            var sql = new Sql();
            sql.Append("SELECT COUNT(*)")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[invoiceKey] = @ikey AND [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @ikey = entityKey, @eckeys = collectionKeys });

            return Database.ExecuteScalar<int>(sql) > 0;
        }

        /// <summary>
        /// Adds a invoice to a static invoice collection.
        /// </summary>
        /// <param name="entityKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(Guid entityKey, Guid collectionKey)
        {
            if (this.ExistsInCollection(entityKey, collectionKey)) return;

            var dto = new Invoice2EntityCollectionDto()
                          {
                              InvoiceKey = entityKey,
                              EntityCollectionKey = collectionKey,
                              CreateDate = DateTime.Now,
                              UpdateDate = DateTime.Now
                          };

            Database.Insert(dto);
        }

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="entityKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(Guid entityKey, Guid collectionKey)
        {
            Database.Execute(
                "DELETE [merchInvoice2EntityCollection] WHERE [merchInvoice2EntityCollection].[invoiceKey] = @ikey AND [merchInvoice2EntityCollection].[entityCollectionKey] = @eckey",
                new { @ikey = entityKey, @eckey = collectionKey });
        }

        /// <summary>
        /// The get invoice keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        public Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets a page of distinct entity keys from entities contained in multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
               .Append("GROUP BY invoiceKey")
               .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = collectionKeys.Count() })
               .Append(")"); 

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        public Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(term);
            sql.Append("AND [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets a page of distinct entity keys from entities contained in multiple collections.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(term);
            sql.Append("AND [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append("GROUP BY invoiceKey")
                .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = collectionKeys.Count() })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        public Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] NOT IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
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
        public Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] NOT IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        public Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(term);
            sql.Append("AND [merchInvoice].[pk] NOT IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        public Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(term);
            sql.Append("AND [merchInvoice].[pk] NOT IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckey = collectionKeys })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        public Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        public Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(term);
            sql.Append("AND [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT([invoiceKey])")
                .Append("FROM [merchInvoice2EntityCollection]")
                .Append(
                    "WHERE [merchInvoice2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckey = collectionKeys })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets invoices from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysFromCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// Gets invoices from collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysThatExistInAllCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IInvoice>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
        }

        /// <summary>
        /// The get from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetFromCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysFromCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// The get from collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysThatExistInAllCollections(collectionKeys, term, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IInvoice>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
        }

        #endregion

        /// <summary>
        /// Gets distinct currency codes used in invoices.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public IEnumerable<string> GetDistinctCurrencyCodes()
        {
            return
                Database.Fetch<DistinctCurrencyCodeDto>("SELECT DISTINCT(currencyCode) from merchInvoice")
                    .Select(x => x.CurrencyCode);
        }

        /// <summary>
        ///  Gets the totals of invoices in a date range for a specific currency code.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <param name="excludeCancelledAndFraud"></param>
        /// <returns>
        /// The sum of the invoice totals.
        /// </returns>
        public decimal SumInvoiceTotals(DateTime startDate, DateTime endDate, string currencyCode, bool excludeCancelledAndFraud = true)
        {
            //var ends = endDate.AddDays(1);
            if (startDate != DateTime.MinValue && startDate != DateTime.MaxValue &&
                endDate != DateTime.MinValue && endDate != DateTime.MaxValue &&
                endDate > startDate)
            {
                var sql = new Sql();

                sql.Append("SELECT SUM([merchInvoice].total) FROM merchInvoice WHERE [merchInvoice].invoiceDate BETWEEN @starts and @ends AND [merchInvoice].currencyCode = @cc", new
                {
                    @starts = startDate.GetStartOfDay(),//.GetDateForSqlStartOfDay(),
                    @ends = endDate.GetEndOfDay(),//.GetDateForSqlEndOfDay(),
                    @cc = currencyCode
                });

                if (excludeCancelledAndFraud)
                {
                    sql.Append("AND [merchInvoice].invoiceStatusKey != '" + Core.Constants.InvoiceStatus.Cancelled.ToString() + "' AND [merchInvoice].invoiceStatusKey != '" + Core.Constants.InvoiceStatus.Fraud.ToString() + "'");
                }

                return Database.ExecuteScalar<decimal>(sql);
            }

            return -1;
        }

        /// <summary>
        ///  Gets the totals of invoices in a date range for a specific currency code.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <param name="invoiceStatuses">
        /// The invoice statuses.
        /// </param>
        /// <returns>
        /// The sum of the invoice totals.
        /// </returns>
        public decimal SumInvoiceTotals(DateTime startDate, DateTime endDate, string currencyCode, IEnumerable<IInvoiceStatus> invoiceStatuses)
        {
            if (startDate != DateTime.MinValue && startDate != DateTime.MaxValue &&
                endDate != DateTime.MinValue && endDate != DateTime.MaxValue &&
                endDate > startDate)
            {
                var sql = new Sql();

                sql.Append("SELECT SUM([merchInvoice].total) FROM merchInvoice WHERE [merchInvoice].invoiceDate BETWEEN @starts and @ends AND [merchInvoice].currencyCode = @cc", new
                {
                    @starts = startDate.GetDateForSqlStartOfDay(),
                    @ends = endDate.GetDateForSqlEndOfDay(),
                    @cc = currencyCode
                });

                if (invoiceStatuses != null && invoiceStatuses.Any())
                {
                    var keys = invoiceStatuses.Select(x => x.Key);
                    sql.Append("AND [merchInvoice].invoiceStatusKey IN (@keys)", new { @keys });
                }

                return Database.ExecuteScalar<decimal>(sql);
            }

            return -1;
        }

        /// <summary>
        /// Gets the total of line items for a give SKU invoiced in a specific currency across the date range.
        /// </summary>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The total of line items for a give SKU invoiced in a specific currency across the date range.
        /// </returns>
        public decimal SumLineItemTotalsBySku(DateTime startDate, DateTime endDate, string currencyCode, string sku)
        {
            const string SQL = @"SELECT	SUM(T2.[quantity] * T2.[price]) AS Total
                        FROM	[merchInvoice] T1
                        INNER JOIN [merchInvoiceItem] T2 ON T1.[pk] = T2.[invoiceKey]
                        WHERE T2.sku = @sku
                        AND T1.currencyCode = @cc
                        AND T1.invoiceDate BETWEEN @starts AND @ends";

            return Database.ExecuteScalar<decimal>(
                SQL,
                new {
                    @starts = startDate.GetStartOfDay(),//.GetDateForSqlStartOfDay(),
                    @ends = endDate.GetEndOfDay(),//.GetDateForSqlEndOfDay(),
                    @cc = currencyCode, @sku = sku
                });
        }

        #region Filter Queries

        /// <summary>
        /// Gets invoices matching the search term and the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetInvoicesMatchingInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetInvoiceKeysMatchingInvoiceStatus(
                searchTerm,
                invoiceStatusKey,
                page,
                itemsPerPage,
                orderExpression,
                sortDirection);


            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// Gets invoice keys matching the search term and the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        public Page<Guid> GetInvoiceKeysMatchingInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(searchTerm);
            sql.Append("AND [merchInvoice].[invoiceStatusKey] = @invsk", new { @invsk = invoiceStatusKey });
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets invoices matching the search term but not the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetInvoicesMatchingTermNotInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetInvoiceKeysMatchingTermNotInvoiceStatus(
                searchTerm,
                invoiceStatusKey,
                page,
                itemsPerPage,
                orderExpression,
                sortDirection);


            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// Gets invoice keys matching the search term but not the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        public Page<Guid> GetInvoiceKeysMatchingTermNotInvoiceStatus(
            string searchTerm,
            Guid invoiceStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(searchTerm);
            sql.Append("AND [merchInvoice].[invoiceStatusKey] != @invsk", new { @invsk = invoiceStatusKey });
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets invoices matching the search term and the order status key.
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetInvoicesMatchingOrderStatus(
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = GetInvoiceKeysMatchingOrderStatus(
                orderStatusKey,
                page,
                itemsPerPage,
                orderExpression,
                sortDirection);

            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// Gets invoice keys matching the search term and the order status key.
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public Page<Guid> GetInvoiceKeysMatchingOrderStatus(
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] IN (")
                .Append("SELECT DISTINCT(invoiceKey)")
                .Append("FROM [merchOrder]")
                .Append("WHERE [merchOrder].[orderStatusKey] = @osk", new { @osk = orderStatusKey })
                .Append(")");

            if (orderStatusKey.Equals(Core.Constants.OrderStatus.NotFulfilled))
            {
                sql.Append("OR [merchInvoice].[pk] NOT IN (");
                sql.Append("SELECT DISTINCT(invoiceKey)");
                sql.Append("FROM [merchOrder]");
                sql.Append(")");
            }

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets invoices matching the search term and the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public Page<IInvoice> GetInvoicesMatchingOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = GetInvoiceKeysMatchingOrderStatus(
                orderStatusKey,
                page,
                itemsPerPage,
                orderExpression,
                sortDirection);

            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// Gets invoice keys matching the search term and the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public Page<Guid> GetInvoiceKeysMatchingOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(searchTerm);
            sql.Append("AND ([merchInvoice].[pk] IN (");
            sql.Append("SELECT DISTINCT(invoiceKey)");
            sql.Append("FROM [merchOrder]");
            sql.Append("WHERE [merchOrder].[orderStatusKey] = @osk", new { @osk = orderStatusKey });
            sql.Append(")");

            if (orderStatusKey.Equals(Core.Constants.OrderStatus.NotFulfilled))
            {
                sql.Append("OR [merchInvoice].[pk] NOT IN (");
                sql.Append("SELECT DISTINCT(invoiceKey)");
                sql.Append("FROM [merchOrder]");
                sql.Append(")");
            }
            sql.Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets invoices matching the search term but not the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        public Page<IInvoice> GetInvoicesMatchingTermNotOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = GetInvoiceKeysMatchingOrderStatus(
                orderStatusKey,
                page,
                itemsPerPage,
                orderExpression,
                sortDirection);

            return new Page<IInvoice>()
                       {
                           CurrentPage = p.CurrentPage,
                           ItemsPerPage = p.ItemsPerPage,
                           TotalItems = p.TotalItems,
                           TotalPages = p.TotalPages,
                           Items = p.Items.Select(Get).ToList()
                       };
        }

        /// <summary>
        /// Gets invoice keys matching the search term but not the order status key.
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public Page<Guid> GetInvoiceKeysMatchingTermNotOrderStatus(
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchInvoice]")
                .Append("WHERE [merchInvoice].[pk] NOT IN (")
                .Append("SELECT DISTINCT(invoiceKey)")
                .Append("FROM [merchOrder]")
                .Append("WHERE [merchOrder].[orderStatusKey] != @osk", new { @osk = orderStatusKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets invoice keys matching the search term but not the order status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public Page<Guid> GetInvoiceKeysMatchingTermNotOrderStatus(
            string searchTerm,
            Guid orderStatusKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = BuildInvoiceSearchSql(searchTerm);
            sql.Append("AND [merchInvoice].[pk] NOT IN (");
            sql.Append("SELECT DISTINCT(invoiceKey)");
            sql.Append("FROM [merchOrder]");
            sql.Append("WHERE [merchOrder].[orderStatusKey] != @osk", new { @osk = orderStatusKey });
            sql.Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        #endregion

        /// <summary>
        /// Gets an <see cref="IInvoice"/>.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IInvoice"/>.
        /// </returns>
        protected override IInvoice PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false).Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(sql).FirstOrDefault();

            if (dto == null) return null;

            var lineItems = GetLineItemCollection(key);
            var orders = GetOrderCollection(key);
            var notes = this.GetNotes(key);
            var factory = new InvoiceFactory(lineItems, orders, notes);
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets a collection of <see cref="IInvoice"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IInvoice"/>.
        /// </returns>
        protected override IEnumerable<IInvoice> PerformGetAll(params Guid[] keys)
        {
            var dtos = new List<InvoiceDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(GetBaseQuery(false).WhereIn<InvoiceDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(GetBaseQuery(false));
            }

            foreach (var dto in dtos)
            {
                var lineItems = GetLineItemCollection(dto.Key);
                var orders = GetOrderCollection(dto.Key);
                var notes = GetNotes(dto.Key);
                var factory = new InvoiceFactory(lineItems, orders, notes);
                yield return factory.BuildEntity(dto);
            }

            // TODO - Keeping original code
            //if (keys.Any())
            //{
            //    foreach (var key in keys)
            //    {
            //        yield return Get(key);
            //    }
            //}
            //else
            //{
            //    var dtos = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(GetBaseQuery(false));
            //    foreach (var dto in dtos)
            //    {
            //        yield return Get(dto.Key);
            //    }
            //}
        }

        /// <summary>
        /// Gets a collection of <see cref="IInvoice"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IInvoice"/>.
        /// </returns>
        protected override IEnumerable<IInvoice> PerformGetByQuery(IQuery<IInvoice> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IInvoice>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<InvoiceDto, InvoiceIndexDto, InvoiceStatusDto>(sql);

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
                .From<InvoiceDto>(SqlSyntax)
                .InnerJoin<InvoiceIndexDto>(SqlSyntax)
                .On<InvoiceDto, InvoiceIndexDto>(SqlSyntax, left => left.Key, right => right.InvoiceKey)
                .InnerJoin<InvoiceStatusDto>(SqlSyntax)
                .On<InvoiceDto, InvoiceStatusDto>(SqlSyntax, left => left.InvoiceStatusKey, right => right.Key);

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
            return "merchInvoice.pk = @Key";
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
                               "DELETE FROM merchNote WHERE entityKey = @Key",
                               "DELETE FROM merchAppliedPayment WHERE invoiceKey = @Key",
                               "DELETE FROM merchInvoiceItem WHERE invoiceKey = @Key",
                               "DELETE FROM merchInvoiceIndex WHERE invoiceKey = @Key",
                               "DELETE FROM merchOfferRedeemed WHERE invoiceKey = @Key",
                               "DELETE FROM merchInvoice2EntityCollection WHERE invoiceKey = @Key",
                               "DELETE FROM merchInvoice WHERE pk = @Key"
                           };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IInvoice entity)
        {

            ((Entity)entity).AddingEntity();

            var factory = new InvoiceFactory(entity.Items, new OrderCollection(), entity.Notes);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;

            SaveNotes(entity);

            Database.Insert(dto.InvoiceIndexDto);
            ((Invoice)entity).ExamineId = dto.InvoiceIndexDto.Id;

            _invoiceLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IInvoice entity)
        {
            SaveNotes(entity);

            ((Entity)entity).UpdatingEntity();

            var factory = new InvoiceFactory(entity.Items, entity.Orders, entity.Notes);
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            _invoiceLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Saves the notes.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void SaveNotes(IInvoice entity)
        {
            var query = Querying.Query<INote>.Builder.Where(x => x.EntityKey == entity.Key);
            var existing = _noteRepository.GetByQuery(query);

            var removers = existing.Where(x => !Guid.Empty.Equals(x.Key) && entity.Notes.All(y => y.Key != x.Key)).ToArray();

            foreach (var remover in removers) _noteRepository.Delete(remover);

            var updates = entity.Notes.Where(x => removers.All(y => y.Key != x.Key));

            var factory = new NoteFactory();
            foreach (var u in updates)
            {
                u.EntityKey = entity.Key;

                if (u.HasIdentity)
                {
                    ((Note)u).UpdatingEntity();
                    var dto = factory.BuildDto(u);
                    Database.Update(dto);
                }
                else
                {
                    ((Note)u).AddingEntity();
                    var dto = factory.BuildDto(u);
                    Database.Insert(dto);
                    u.Key = dto.Key;
                }
            }

        }

        /// <summary>
        /// Builds an invoice search query.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        private Sql BuildInvoiceSearchSql(string searchTerm)
        {
            searchTerm = searchTerm.Replace(",", " ");
            var invidualTerms = searchTerm.Split(' ');

            var numbers = new List<int>();
            var terms = new List<string>();

            foreach (var term in invidualTerms.Where(x => !string.IsNullOrEmpty(x)))
            {
                int invoiceNumber;
                if (int.TryParse(term, out invoiceNumber))
                {
                    numbers.Add(invoiceNumber);
                }
                else
                {
                    terms.Add(term);
                }
            }


            var sql = new Sql();

            sql.Select("*").From<InvoiceDto>(SqlSyntax);

            if (numbers.Any() && terms.Any())
            {
                sql.Where(
                    "billToName LIKE @term OR billToEmail LIKE @email OR poNumber LIKE @ponumber OR billToAddress1 LIKE @adr1 OR billToLocality LIKE @loc OR invoiceNumber IN (@invNo) OR billToPostalCode IN (@postal)",
                    new
                        {
                            @term = string.Format("%{0}%", string.Join("% ", terms)).Trim(),
                            @email = string.Format("%{0}%", string.Join("% ", terms)).Trim(),
                            @adr1 = string.Format("%{0}%", string.Join("%", terms)).Trim(),
                            @loc = string.Format("%{0}%", string.Join("%", terms)).Trim(),
                            @invNo = numbers.ToArray(),
                            @postal = string.Format("%{0}%", string.Join("%", terms)).Trim(),
                            @ponumber = string.Format("%{0}%", string.Join("% ", terms)).Trim()
                    });
            }
            else if (numbers.Any())
            {
				if (numbers.Count() == 1)
				{
					int number = numbers[0];
					// if there is only one number, use starts-with type logic so that the list descreases as more digits are inserted.
					// invoiceNumber is indexed, so use index by including ranges. Query looks ugly, but more effectcient than casting
					// invoiceNumber to a string and using a 'like' - which wouldn't use an index.
					// postcode is a string and not indexed - so is doing a full table scan. If performance is an issue on large data sets,
					//   consider removing postcode from lookup or enhancing with an index
					sql.Where(
						"invoiceNumber = @invNo OR invoiceNumber BETWEEN @invNo10 AND @invNo19 OR invoiceNumber BETWEEN @invNo100 AND @invNo199 OR invoiceNumber BETWEEN @invNo1000 AND @invNo1999 OR invoiceNumber BETWEEN @invNo10000 AND @invNo19999 OR invoiceNumber BETWEEN @invNo100000 AND @invNo199999 OR invoiceNumber BETWEEN @invNo1000000 AND @invNo1999999 OR billToPostalCode LIKE @postal ", 
						new {
							@invNo = number,
							@invNo10 = number * 10,
							@invNo19 = number * 10 + 9,
							@invNo100 = number * 100,
							@invNo199 = number * 100 + 99,
							@invNo1000 = number * 1000,
							@invNo1999 = number * 1000 + 999,
							@invNo10000 = number * 10000,
							@invNo19999 = number * 10000 + 9999,
							@invNo100000 = number * 100000,
							@invNo199999 = number * 100000 + 99999,
							@invNo1000000 = number * 1000000,
							@invNo1999999 = number * 1000000 + 999999,
							@postal = string.Format("{0}%", number).Trim()
						});
				}
				else
				{
					sql.Where("invoiceNumber IN (@invNo) OR billToPostalCode IN (@postal) ", new { @invNo = numbers.ToArray(), @postal = numbers.ToArray() });
				}
            }
            else
            {
                sql.Where(
                    "billToName LIKE @term OR billToEmail LIKE @term OR poNumber LIKE @term OR billToAddress1 LIKE @adr1 OR billToLocality LIKE @loc OR billToPostalCode IN (@postal)",
                    new
                        {
                            @term = string.Format("%{0}%", string.Join("% ", terms)).Trim(),
                            @email = string.Format("%{0}%", string.Join("% ", terms)).Trim(),
                            @adr1 = string.Format("%{0}%", string.Join("%", terms)).Trim(),
                            @loc = string.Format("%{0}%", string.Join("%", terms)).Trim(),
                            @postal = string.Format("%{0}%", string.Join("%", terms)).Trim()
                    });
            }

            return sql;
        }

        /// <summary>
        /// The get line item collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="LineItemCollection"/>.
        /// </returns>
        private LineItemCollection GetLineItemCollection(Guid invoiceKey)
        {
            var sql = new Sql();
            sql.Select("*").From<InvoiceItemDto>(SqlSyntax).Where<InvoiceItemDto>(x => x.ContainerKey == invoiceKey);

            var dtos = Database.Fetch<InvoiceItemDto>(sql);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        /// <summary>
        /// The get order collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="OrderCollection"/>.
        /// </returns>
        private OrderCollection GetOrderCollection(Guid invoiceKey)
        {
            var query = Querying.Query<IOrder>.Builder.Where(x => x.InvoiceKey == invoiceKey);
            var orders = _orderRepository.GetByQuery(query);
            var collection = new OrderCollection();

            foreach (var order in orders)
            {
                collection.Add(order);
            }

            return collection;
        }

        /// <summary>
        /// Gets the notes collection for an invoice.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INote}"/>.
        /// </returns>
        private IEnumerable<INote> GetNotes(Guid invoiceKey)
        {
            var query = Querying.Query<INote>.Builder.Where(x => x.EntityKey == invoiceKey && x.EntityTfKey == Core.Constants.TypeFieldKeys.Entity.InvoiceKey);
            var notes = _noteRepository.GetByQuery(query);

            var collection = new List<INote>();

            foreach (var note in notes.OrderByDescending(x => x.CreateDate))
            {
                collection.Add(note);
            }

            return collection;   
        }
    }
}