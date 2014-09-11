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
    /// The customer repository.
    /// </summary>
    internal class CustomerRepository : PagedRepositoryBase<ICustomer, CustomerDto>, ICustomerRepository
    {
        /// <summary>
        /// The _customer address repository.
        /// </summary>
        private readonly ICustomerAddressRepository _customerAddressRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The database unit of work
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="customerAddressRepository">
        /// The customer Address Repository.
        /// </param>
        public CustomerRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ICustomerAddressRepository customerAddressRepository) 
            : base(work, cache)
        {
            Mandate.ParameterNotNull(customerAddressRepository, "customerAddressRepository");

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
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();            

            var sql = new Sql();
            sql.Select("*").From<CustomerDto>();

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("%", terms));

                sql.Where("lastName LIKE @ln OR firstName LIKE @fn OR email LIKE @email", new { @ln = preparedTerms, @fn = preparedTerms, @email = preparedTerms });
            }

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
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

            var customer = factory.BuildEntity(dto, _customerAddressRepository.GetByCustomerKey(key));

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
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {
                var factory = new CustomerFactory();
                var dtos = Database.Fetch<CustomerDto, CustomerIndexDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {                    
                    yield return factory.BuildEntity(dto, _customerAddressRepository.GetByCustomerKey(dto.Key));
                }
            }
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
                .From<CustomerDto>()
                .InnerJoin<CustomerIndexDto>()
                .On<CustomerDto, CustomerIndexDto>(left => left.Key, right => right.CustomerKey);

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
                    "DELETE FROM merchItemCacheItem WHERE ItemCacheKey IN (SELECT pk FROM merchItemCache WHERE entityKey = @Key)",
                    "DELETE FROM merchItemCache WHERE entityKey = @Key",
                    "DELETE FROM merchCustomerAddress WHERE customerKey = @Key",
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
    }
}
