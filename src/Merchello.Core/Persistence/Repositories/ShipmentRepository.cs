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
    internal class ShipmentRepository : MerchelloPetaPocoRepositoryBase<int, IShipment>, IShipmentRepository
    {

        public ShipmentRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public ShipmentRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IShipment>


        protected override IShipment PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ShipmentDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipmentFactory();

            var shipment = factory.BuildEntity(dto);

            return shipment;
        }

        protected override IEnumerable<IShipment> PerformGetAll(params int[] ids)
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
                var factory = new ShipmentFactory();
                var dtos = Database.Fetch<ShipmentDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IShipment>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ShipmentDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchShipment.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchShipment WHERE id = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IShipment entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new ShipmentFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IShipment entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new ShipmentFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IShipment entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IShipment> PerformGetByQuery(IQuery<IShipment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipmentDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
