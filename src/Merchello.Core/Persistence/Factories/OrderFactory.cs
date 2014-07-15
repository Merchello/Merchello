namespace Merchello.Core.Persistence.Factories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// The order factory.
    /// </summary>
    internal class OrderFactory : IEntityFactory<IOrder, OrderDto>
    {
        /// <summary>
        /// The line item collection.
        /// </summary>
        private readonly LineItemCollection _lineItemCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFactory"/> class.
        /// </summary>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        public OrderFactory(LineItemCollection lineItemCollection)
        {
            _lineItemCollection = lineItemCollection;
        }

        /// <summary>
        /// Builds an order entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IOrder"/>.
        /// </returns>
        public IOrder BuildEntity(OrderDto dto)
        {
            var factory = new OrderStatusFactory();
            var order = new Order(factory.BuildEntity(dto.OrderStatusDto), dto.InvoiceKey, _lineItemCollection)
                {
                    Key = dto.Key,
                    OrderNumberPrefix = dto.OrderNumberPrefix,
                    OrderNumber = dto.OrderNumber,
                    OrderDate = dto.OrderDate,
                    VersionKey = dto.VersionKey,
                    Exported = dto.Exported,
                    ExamineId = dto.OrderIndexDto.Id,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };

            order.ResetDirtyProperties();

            return order;
        }

        /// <summary>
        /// Builds an order dto
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="OrderDto"/>.
        /// </returns>
        public OrderDto BuildDto(IOrder entity)
        {
            return new OrderDto()
                {
                    Key = entity.Key,
                    InvoiceKey = entity.InvoiceKey,
                    OrderNumberPrefix = entity.OrderNumberPrefix,
                    OrderNumber = entity.OrderNumber,
                    OrderStatusKey = entity.OrderStatusKey,
                    OrderDate = entity.OrderDate,
                    VersionKey = entity.VersionKey,
                    Exported = entity.Exported,
                    OrderIndexDto = new OrderIndexDto()
                    {
                        Id = ((Order)entity).ExamineId,
                        OrderKey = entity.Key,
                        UpdateDate = entity.UpdateDate,
                        CreateDate = entity.CreateDate
                    },
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}