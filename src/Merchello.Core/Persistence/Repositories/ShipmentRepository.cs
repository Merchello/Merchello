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
    /// The shipment repository.
    /// </summary>
    internal class ShipmentRepository : MerchelloPetaPocoRepositoryBase<IShipment>, IShipmentRepository
    {
        private readonly IOrderLineItemRepository _orderLineItemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="orderLineItemRepository">
        /// The order Line Item Repository.
        /// </param>
        public ShipmentRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, IOrderLineItemRepository orderLineItemRepository)
            : base(work, cache)
        {
            Mandate.ParameterNotNull(orderLineItemRepository, "orderLineItemRepository");
            _orderLineItemRepository = orderLineItemRepository;
        }

        #region Overrides of RepositoryBase<IShipment>


        protected override IShipment PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipmentDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipmentFactory();

            var shipment = factory.BuildEntity(dto);

            return shipment;
        }

        protected override IEnumerable<IShipment> PerformGetAll(params Guid[] keys)
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
            return "merchShipment.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "UPDATE merchOrderLineItem SET shipmentKey = NULL WHERE shipmentKey = @Key",
                    "DELETE FROM merchShipment WHERE pk = @Key",
                };

            return list;
        }

        protected override void PersistNewItem(IShipment entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ShipmentFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Key = dto.Key;

            foreach (var item in entity.Items.ToArray())
            {
                ((IOrderLineItem)item).ShipmentKey = entity.Key;
                _orderLineItemRepository.SaveLineItem((IOrderLineItem)item);
            }

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IShipment entity)
        {
            ((Entity)entity).UpdatingEntity();

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
                Database.Execute(delete, new { Key = entity.Key });
            }
        }


        protected override IEnumerable<IShipment> PerformGetByQuery(IQuery<IShipment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipmentDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }

        #endregion

    }
}
