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
    /// The shipment repository.
    /// </summary>
    internal class ShipmentRepository : MerchelloPetaPocoRepositoryBase<IShipment>, IShipmentRepository
    {
        /// <summary>
        /// The order line item repository.
        /// </summary>
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
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        public ShipmentRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, IOrderLineItemRepository orderLineItemRepository, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(orderLineItemRepository, "orderLineItemRepository");
            _orderLineItemRepository = orderLineItemRepository;
        }


        /// <summary>
        /// The get max document number.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaxDocumentNumber()
        {
            var value = Database.ExecuteScalar<object>("SELECT TOP 1 shipmentNumber FROM merchShipment ORDER BY shipmentNumber DESC");
            return value == null ? 0 : int.Parse(value.ToString());
        }


        /// <summary>
        /// Gets a shipment by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        protected override IShipment PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ShipmentDto, ShipmentStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ShipmentFactory();

            var shipment = factory.BuildEntity(dto);
            ((Shipment)shipment).Items = this.GetLineItems(dto.Key);
            return shipment;
        }

        /// <summary>
        /// Gets a collection of all shipments with the option to pass an array of shipment keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IShipment}"/>.
        /// </returns>
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
                var dtos = Database.Fetch<ShipmentDto, ShipmentStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return this.Get(dto.Key);
                }
            }
        }

        /// <summary>
        /// Constructs the base shipment query
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
                .From<ShipmentDto>(SqlSyntax)
                .InnerJoin<ShipmentStatusDto>(SqlSyntax)
                .On<ShipmentDto, ShipmentStatusDto>(SqlSyntax, left => left.ShipmentStatusKey, right => right.Key);

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
            return "merchShipment.pk = @Key";
        }

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            // TODO RSS : need to refactor this to remove the "UPDATE" this should be done in a Delete override
            var list = new List<string>
                {
                    "UPDATE merchOrderItem SET shipmentKey = NULL WHERE shipmentKey = @Key",
                    "DELETE FROM merchShipment WHERE pk = @Key",
                };

            return list;
        }

        /// <summary>
        /// Persists a new shipment record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
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

        /// <summary>
        /// Persists an updated shipment record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IShipment entity)
        {
            var query = Querying.Query<IOrderLineItem>.Builder.Where(x => x.ShipmentKey == entity.Key);
            var existing = _orderLineItemRepository.GetByQuery(query);

            var removers = existing.Where(x => entity.Items.All(y => y.Key != x.Key));
                
            foreach (var remove in removers)
            {
                ((IOrderLineItem)remove).ShipmentKey = null;
                _orderLineItemRepository.SaveLineItem((IOrderLineItem)remove);
            }

            ((Entity)entity).UpdatingEntity();

            var factory = new ShipmentFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Deletes a shipment record.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IShipment entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// Gets a collection of shipments by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of shipments.
        /// </returns>
        protected override IEnumerable<IShipment> PerformGetByQuery(IQuery<IShipment> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IShipment>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ShipmentDto, ShipmentStatusDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets a collection of line items for a shipment.
        /// </summary>
        /// <param name="shipmentKey">
        /// The shipment key.
        /// </param>
        /// <returns>
        /// The <see cref="LineItemCollection"/>.
        /// </returns>
        private LineItemCollection GetLineItems(Guid shipmentKey)
        {
            var query = Querying.Query<IOrderLineItem>.Builder.Where(x => x.ShipmentKey == shipmentKey);
            var items = _orderLineItemRepository.GetByQuery(query);

            return new LineItemCollection { items };
        }
    }
}
