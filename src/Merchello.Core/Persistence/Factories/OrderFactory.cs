using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class OrderFactory : IEntityFactory<IOrder, OrderDto>
    {
        private readonly LineItemCollection _lineItemCollection;

        public OrderFactory(LineItemCollection lineItemCollection)
        {
            _lineItemCollection = lineItemCollection;
        }

        public IOrder BuildEntity(OrderDto dto)
        {
            var order = new Order(dto.OrderStatusKey, dto.InvoiceKey, _lineItemCollection)
                {
                    Key =  dto.Key,
                    OrderNumberPrefix = dto.OrderNumberPrefix,
                    OrderNumber = dto.OrderNumber,
                    OrderDate = dto.OrderDate,
                    VersionKey = dto.VersionKey,
                    Exported = dto.Exported,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };

            order.ResetDirtyProperties();

            return order;
        }

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
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}