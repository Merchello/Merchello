using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class WarehouseRepository : MerchelloPetaPocoRepositoryBase<int, IWarehouse>, IWarehouseRepository
    {

        public WarehouseRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public WarehouseRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IWarehouse>


        protected override IWarehouse PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<WarehouseDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new WarehouseFactory();

            var warehouse = factory.BuildEntity(dto);

            return warehouse;
        }

        protected override IEnumerable<IWarehouse> PerformGetAll(params int[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new WarehouseFactory();
                var dtos = Database.Fetch<WarehouseDto>(GetBaseQuery(false));
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
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<WarehouseDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchWarehouse.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchWarehouse WHERE WarehousePk = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IWarehouse entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new WarehouseFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IWarehouse entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new WarehouseFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IWarehouse entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IWarehouse> PerformGetByQuery(IQuery<IWarehouse> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IWarehouse>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<WarehouseDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion

    }
}
