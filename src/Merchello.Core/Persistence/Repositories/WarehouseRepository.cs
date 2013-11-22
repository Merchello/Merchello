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

namespace Merchello.Core.Persistence.Repositories
{
    internal class WarehouseRepository : MerchelloPetaPocoRepositoryBase<IWarehouse>, IWarehouseRepository
    {


        public WarehouseRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IWarehouse>


        protected override IWarehouse PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<WarehouseDto, WarehouseCatalogDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new WarehouseFactory();

            var warehouse = factory.BuildEntity(dto);


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
                var dtos = Database.Fetch<WarehouseDto, WarehouseCatalogDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IWarehouse>


        protected override Sql GetBaseQuery(bool isCount)
        {
            // TODO VERSION NEXT: this will need to be refactored when we open up Multiple Warehouses
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<WarehouseDto>()
                .InnerJoin<WarehouseCatalogDto>()
                .On<WarehouseDto, WarehouseCatalogDto>(left => left.Key, right => right.WarehouseKey);

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

            var dtos = Database.Fetch<WarehouseDto, WarehouseCatalogDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }


        #endregion

    }
}
