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
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
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
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="warehouseCatalogRepository">
        /// The warehouse Catalog Repository.
        /// </param>
        public WarehouseRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, IWarehouseCatalogRepository warehouseCatalogRepository)
            : base(work, cache)
        {
            Mandate.ParameterNotNull(warehouseCatalogRepository, "warehouseCatalogRepository");

            _warehouseCatalogRepository = warehouseCatalogRepository;
        }
       
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

        protected override IEnumerable<IWarehouse> PerformGetAll(params Guid[] keys)
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
                var factory = new WarehouseFactory();
                var dtos = Database.Fetch<WarehouseDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto, _warehouseCatalogRepository.GetWarehouseCatalogsByWarehouseKey(dto.Key));
                }
            }
        }


        protected override Sql GetBaseQuery(bool isCount)
        {
            // TODO VERSION NEXT: this will need to be refactored when we open up Multiple Warehouse Catalogs!!!
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<WarehouseDto>();
            
            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchWarehouse.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {            
            var list = new List<string>();
                //{

                //    "DELETE FROM merchWarehouse WHERE pk = @Key",
                //};

            return list;
        }

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

        protected override void PersistUpdatedItem(IWarehouse entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new WarehouseFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IWarehouse entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }


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
