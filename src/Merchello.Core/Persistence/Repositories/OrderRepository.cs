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
    /// Represents the OrderRepository
    /// </summary>
    internal class OrderRepository : PagedRepositoryBase<IOrder, OrderDto>, IOrderRepository
    {
        /// <summary>
        /// The order line item repository.
        /// </summary>
        private readonly ILineItemRepositoryBase<IOrderLineItem> _orderLineItemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="orderLineItemRepository">
        /// The order line item repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public OrderRepository(IDatabaseUnitOfWork work, ILineItemRepositoryBase<IOrderLineItem> orderLineItemRepository, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(orderLineItemRepository, "lineItemRepository");

            _orderLineItemRepository = orderLineItemRepository;
        }

        public override Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a <see cref="IOrder"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOrder"/>.
        /// </returns>
        protected override IOrder PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var lineItems = GetLineItemCollection(key);

            var factory = new OrderFactory(lineItems);
            return factory.BuildEntity(dto);
        }

        /// <summary>
        /// Gets all <see cref="IOrder"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOrder}"/>.
        /// </returns>
        protected override IEnumerable<IOrder> PerformGetAll(params Guid[] keys)
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
                var dtos = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IOrder"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOrder}"/>.
        /// </returns>
        protected override IEnumerable<IOrder> PerformGetByQuery(IQuery<IOrder> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IOrder>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<OrderDto, OrderIndexDto, OrderStatusDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL query.
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
               .From<OrderDto>(SqlSyntax)
               .InnerJoin<OrderIndexDto>(SqlSyntax)
               .On<OrderDto, OrderIndexDto>(SqlSyntax, left => left.Key, right => right.OrderKey)
               .InnerJoin<OrderStatusDto>(SqlSyntax)
               .On<OrderDto, OrderStatusDto>(SqlSyntax, left => left.OrderStatusKey, right => right.Key);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchOrder.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {                
                "DELETE FROM merchOrderItem WHERE orderKey = @Key",
                "DELETE FROM merchOrderIndex WHERE orderKey = @Key",
                "DELETE FROM merchOrder WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Saves a new item to the databse.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(IOrder entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new OrderFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);            
            entity.Key = dto.Key;

            Database.Insert(dto.OrderIndexDto);
            ((Order)entity).ExamineId = dto.OrderIndexDto.Id;


            _orderLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IOrder entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new OrderFactory(entity.Items);
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            _orderLineItemRepository.SaveLineItem(entity.Items, entity.Key);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IOrder entity)
        {
            base.PersistDeletedItem(entity);
        }

        private LineItemCollection GetLineItemCollection(Guid orderKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<OrderItemDto>()
                .Where<OrderItemDto>(x => x.ContainerKey == orderKey);

            var dtos = Database.Fetch<OrderItemDto>(sql);

            var factory = new LineItemFactory();
            var collection = new LineItemCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto));
            }

            return collection;
        }

        /// <summary>
        /// The get max document number.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetMaxDocumentNumber()
        {

            var value = Database.ExecuteScalar<object>("SELECT TOP 1 orderNumber FROM merchOrder ORDER BY orderNumber DESC");
            return value == null ? 0 : int.Parse(value.ToString());
        }
    }
}