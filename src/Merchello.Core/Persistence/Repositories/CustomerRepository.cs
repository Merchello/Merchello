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
    /// The customer repository.
    /// </summary>
    internal class CustomerRepository : PagedRepositoryBase<ICustomer, CustomerDto>, ICustomerRepository
    {
        /// <summary>
        /// The _customer address repository.
        /// </summary>
        private readonly ICustomerAddressRepository _customerAddressRepository;

        /// <summary>
        /// The note repository.
        /// </summary>
        private readonly INoteRepository _noteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The database unit of work
        /// </param>
        /// <param name="customerAddressRepository">
        /// The customer Address Repository.
        /// </param>
        /// <param name="noteRepository">
        /// The note Repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        public CustomerRepository(
            IDatabaseUnitOfWork work, 
            ICustomerAddressRepository customerAddressRepository, 
            INoteRepository noteRepository,
            ILogger logger, 
            ISqlSyntaxProvider sqlSyntax) 
            : base(work, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(customerAddressRepository, "customerAddressRepository");
            Mandate.ParameterNotNull(noteRepository, "noteRepository");

            _noteRepository = noteRepository;
            _customerAddressRepository = customerAddressRepository;
        }

        /// <summary>
        /// Searches customers
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
            var sql = this.BuildCustomerSearchSql(searchTerm);

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Returns a value indicating whether or not the entity exists in a collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
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
                .Append("FROM [merchCustomer2EntityCollection]")
                .Append(
                    "WHERE [merchCustomer2EntityCollection].[customerKey] = @ekey AND [merchCustomer2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @ekey = entityKey, @eckey = collectionKey });

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
                .Append("FROM [merchCustomer2EntityCollection]")
                .Append(
                    "WHERE [merchCustomer2EntityCollection].[customerKey] = @ekey AND [merchCustomer2EntityCollection].[entityCollectionKey] IN(@eckeys)",
                    new { @ekey = entityKey, @eckeys = collectionKeys });

            return Database.ExecuteScalar<int>(sql) > 0;
        }

        /// <summary>
        /// Adds a entity to a static invoice collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(Guid entityKey, Guid collectionKey)
        {
            if (this.ExistsInCollection(entityKey, collectionKey)) return;

            var dto = new Customer2EntityCollectionDto()
            {
                CustomerKey = entityKey,
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
            "DELETE [merchCustomer2EntityCollection] WHERE [merchCustomer2EntityCollection].[customerKey] = @ekey AND [merchCustomer2EntityCollection].[entityCollectionKey] = @eckey",
            new { @ekey = entityKey, @eckey = collectionKey });
        }

        /// <summary>
        /// The get entity keys from collection.
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
              .Append("FROM [merchCustomer]")
               .Append("WHERE [merchCustomer].[pk] IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] = @eckey", new { @eckey = collectionKey })
               .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get entity keys from collection.
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
              .Append("FROM [merchCustomer]")
               .Append("WHERE [merchCustomer].[pk] IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append("GROUP BY customerKey")
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
            var sql = this.BuildCustomerSearchSql(term);
            sql.Append("AND [merchCustomer].[pk] IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] = @eckey", new { @eckey = collectionKey })
               .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys from collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = this.BuildCustomerSearchSql(term);
            sql.Append("AND [merchCustomer].[pk] IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append("GROUP BY customerKey")
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
              .Append("FROM [merchCustomer]")
               .Append("WHERE [merchCustomer].[pk] NOT IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] = @eckey", new { @eckey = collectionKey })
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
              .Append("FROM [merchCustomer]")
               .Append("WHERE [merchCustomer].[pk] NOT IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
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
            var sql = this.BuildCustomerSearchSql(term);
            sql.Append("AND [merchCustomer].[pk] NOT IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] = @eckey", new { @eckey = collectionKey })
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
            var sql = this.BuildCustomerSearchSql(term);
            sql.Append("AND [merchCustomer].[pk] NOT IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
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
              .Append("FROM [merchCustomer]")
               .Append("WHERE [merchCustomer].[pk] IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
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
            var sql = this.BuildCustomerSearchSql(term);
            sql.Append("AND [merchCustomer].[pk] IN (")
               .Append("SELECT DISTINCT([customerKey])")
               .Append("FROM [merchCustomer2EntityCollection]")
               .Append("WHERE [merchCustomer2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Gets entity from collection.
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
        public Page<ICustomer> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysFromCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<ICustomer>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
        }

        /// <summary>
        /// Gets entity from collection.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        public Page<ICustomer> GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysThatExistInAllCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<ICustomer>()
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<ICustomer> GetFromCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysFromCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection);
            return new Page<ICustomer>()
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<ICustomer> GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysThatExistInAllCollections(collectionKeys, term, page, itemsPerPage, orderExpression, sortDirection);
            return new Page<ICustomer>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
        }

        /// <summary>
        /// Performs the Get by key operation.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomer"/>.
        /// </returns>
        protected override ICustomer PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });


            var dto = Database.Fetch<CustomerDto, CustomerIndexDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerFactory();
            var customer = factory.BuildEntity(dto, _customerAddressRepository.GetByCustomerKey(key), GetNotes(key));

            return customer;
        }

        /// <summary>
        /// The perform get all operation.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The collection of all <see cref="ICustomer"/>.
        /// </returns>
        protected override IEnumerable<ICustomer> PerformGetAll(params Guid[] keys)
        {

            var dtos = new List<CustomerDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<CustomerDto, CustomerIndexDto>(GetBaseQuery(false).WhereIn<CustomerDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<CustomerDto, CustomerIndexDto>(GetBaseQuery(false));
            }

            var factory = new CustomerFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto, _customerAddressRepository.GetByCustomerKey(dto.Key), GetNotes(dto.Key));
            }


            // TODO - Not sure if the above is correct so keeping original query
            //if (keys.Any())
            //{
            //    foreach (var key in keys)
            //    {
            //        yield return Get(key);
            //    }
            //}
            //else
            //{
            //    var factory = new CustomerFactory();
            //    var dtos = Database.Fetch<CustomerDto, CustomerIndexDto>(GetBaseQuery(false));
            //    foreach (var dto in dtos)
            //    {                    
            //        yield return factory.BuildEntity(dto, _customerAddressRepository.GetByCustomerKey(dto.Key), GetNotes(dto.Key));
            //    }
            //}
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
                .From<CustomerDto>(SqlSyntax)
                .InnerJoin<CustomerIndexDto>(SqlSyntax)
                .On<CustomerDto, CustomerIndexDto>(SqlSyntax, left => left.Key, right => right.CustomerKey);

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
            return "merchCustomer.pk = @Key";
        }

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchNote WHERE entityKey = @Key",
                    "DELETE FROM merchItemCacheItem WHERE ItemCacheKey IN (SELECT pk FROM merchItemCache WHERE entityKey = @Key)",
                    "DELETE FROM merchItemCache WHERE entityKey = @Key",
                    "DELETE FROM merchCustomerAddress WHERE customerKey = @Key",
                    "DELETE FROM merchCustomer2EntityCollection WHERE customerKey = @Key",
                    "DELETE FROM merchCustomerIndex WHERE customerKey = @Key",
                    "DELETE FROM merchCustomer WHERE pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(ICustomer entity)
        {
            ((Customer)entity).AddingEntity();

            var factory = new CustomerFactory();
            var dto = factory.BuildDto(entity);
            
            Database.Insert(dto);
            entity.Key = dto.Key;

            Database.Insert(dto.CustomerIndexDto);
            ((Customer)entity).ExamineId = dto.CustomerIndexDto.Id;

            SaveNotes(entity);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(ICustomer entity)
        {
            SaveNotes(entity);

            ((Entity)entity).UpdatingEntity();

            var factory = new CustomerFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(ICustomer entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// The perform get by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of <see cref="ICustomer"/>
        /// </returns>
        protected override IEnumerable<ICustomer> PerformGetByQuery(IQuery<ICustomer> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomer>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerDto, CustomerIndexDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Builds customer search SQL.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        private Sql BuildCustomerSearchSql(string searchTerm)
        {
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();

            var sql = new Sql();
            sql.Select("*").From<CustomerDto>(SqlSyntax);

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("% ", terms)).Trim();

                sql.Where("lastName LIKE @ln OR firstName LIKE @fn OR email LIKE @email", new { @ln = preparedTerms, @fn = preparedTerms, @email = preparedTerms });
            }

            return sql;
        }

        /// <summary>
        /// Saves the notes.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        private void SaveNotes(ICustomer entity)
        {
            var query = Querying.Query<INote>.Builder.Where(x => x.EntityKey == entity.Key && x.EntityTfKey == Core.Constants.TypeFieldKeys.Entity.CustomerKey);
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
        /// Gets the notes collection for an invoice.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INote}"/>.
        /// </returns>
        private IEnumerable<INote> GetNotes(Guid customerKey)
        {
            var query = Querying.Query<INote>.Builder.Where(x => x.EntityKey == customerKey && x.EntityTfKey == Core.Constants.TypeFieldKeys.Entity.CustomerKey);
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
