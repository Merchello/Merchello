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
    internal class ShipMethodRepository : MerchelloPetaPocoRepositoryBase<int, IShipMethod>, IShipMethodRepository
    {

        public ShipMethodRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public ShipMethodRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IShipMethod>


        protected override IShipMethod PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ShipMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipMethodFactory();

            var shipMethod = factory.BuildEntity(dto);

            return shipMethod;
        }

        protected override IEnumerable<IShipMethod> PerformGetAll(params int[] ids)
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
                var factory = new ShipMethodFactory();
                var dtos = Database.Fetch<ShipMethodDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IShipMethod>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ShipMethodDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchShipMethod.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchShipMethod2Warehouse WHERE shipMethodId = @Id",
                    "UPDATE merchShipment SET shipMethodId = null WHERE shipMethodId = @Id",
                    "DELETE FROM merchShipMethod WHERE id = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IShipMethod entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new ShipMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IShipMethod entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new ShipMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IShipMethod entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IShipMethod> PerformGetByQuery(IQuery<IShipMethod> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipMethodDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
