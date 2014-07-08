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

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    using IDatabaseUnitOfWork = Merchello.Core.Persistence.UnitOfWork.IDatabaseUnitOfWork;

    /// <summary>
    /// The customer address repository.
    /// </summary>
    internal class CustomerAddressRepository : MerchelloPetaPocoRepositoryBase<ICustomerAddress>, ICustomerAddressRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public CustomerAddressRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        /// <summary>
        /// The perform get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICustomerAddress"/>.
        /// </returns>
        protected override ICustomerAddress PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<CustomerAddressDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CustomerAddressFactory();

            var address = factory.BuildEntity(dto);

            return address;
        }

        /// <summary>
        /// The perform get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The a collection of all customer addresses
        /// </returns>
        protected override IEnumerable<ICustomerAddress> PerformGetAll(params Guid[] keys)
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
                var factory = new CustomerAddressFactory();
                var dtos = Database.Fetch<CustomerAddressDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
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
               .From("merchAddress");

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
            return "merchAddress.pk = @Key";
        }

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchAddress WHERE pk = @Key",
                };

            return list;
        }

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity to be created
        /// </param>
        protected override void PersistNewItem(ICustomerAddress entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new CustomerAddressFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity to be updated
        /// </param>
        protected override void PersistUpdatedItem(ICustomerAddress entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CustomerAddressFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity to be deleted
        /// </param>
        protected override void PersistDeletedItem(ICustomerAddress entity)
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
        /// The collection of <see cref="ICustomerAddress"/>.
        /// </returns>
        protected override IEnumerable<ICustomerAddress> PerformGetByQuery(IQuery<ICustomerAddress> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICustomerAddress>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CustomerAddressDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }
    }
}
