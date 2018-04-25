namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Factories;
    using Models;
    using Models.EntityBase;
    using Models.Rdbms;
    using Querying;
    
    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    using UnitOfWork;

    /// <summary>
    /// The warehouse repository.
    /// </summary>
    internal class WarehouseRepository : MerchelloPetaPocoRepositoryBase<IWarehouse>, IWarehouseRepository
    {
        /// <summary>
        /// The _warehouse catalog repository.
        /// </summary>
        private readonly IWarehouseCatalogRepository _warehouseCatalogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="warehouseCatalogRepository">
        /// The warehouse Catalog Repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        public WarehouseRepository(IDatabaseUnitOfWork work, IWarehouseCatalogRepository warehouseCatalogRepository, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(warehouseCatalogRepository, "warehouseCatalogRepository");

            _warehouseCatalogRepository = warehouseCatalogRepository;
        }

        /// <summary>
        /// Gets a <see cref="IWarehouse"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouse"/>.
        /// </returns>
        protected override IWarehouse PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<WarehouseDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new WarehouseFactory();

            var warehouse = factory.BuildEntity(dto, _warehouseCatalogRepository.GetWarehouseCatalogsByWarehouseKey(key));


            return warehouse;
        }

        /// <summary>
        /// Gets the collection of all <see cref="IWarehouse"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IWarehouse}"/>.
        /// </returns>
        protected override IEnumerable<IWarehouse> PerformGetAll(params Guid[] keys)
        {

            var dtos = new List<WarehouseDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<WarehouseDto>(GetBaseQuery(false).WhereIn<WarehouseDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<WarehouseDto>(GetBaseQuery(false));
            }

            var factory = new WarehouseFactory();
            foreach (var dto in dtos)
            {
                yield return factory.BuildEntity(dto, _warehouseCatalogRepository.GetWarehouseCatalogsByWarehouseKey(dto.Key));
            }
        }

        /// <summary>
        /// Gets the base SQL clause.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            // TODO VERSION NEXT: this will need to be refactored when we open up Multiple Warehouse Catalogs
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<WarehouseDto>(SqlSyntax);
            
            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="String"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchWarehouse.pk = @Key";
        }

        /// <summary>
        /// Gets a list delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {            
            var list = new List<string>();
                //{

                //    "DELETE FROM merchWarehouse WHERE pk = @Key",
                //};

            return list;
        }

        /// <summary>
        /// Saves a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IWarehouse entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new WarehouseFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;

            // TODO : warehouses will need to have a default WarehouseCatalog

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IWarehouse entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new WarehouseFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Deletes an existing item from the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IWarehouse entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IWarehouse"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IWarehouse}"/>.
        /// </returns>
        protected override IEnumerable<IWarehouse> PerformGetByQuery(IQuery<IWarehouse> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IWarehouse>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<WarehouseDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }
    }
}
