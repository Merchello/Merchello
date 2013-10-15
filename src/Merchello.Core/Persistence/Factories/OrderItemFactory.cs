using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class OrderItemFactory : ILineItemFactory<OrderItemDto>
    {
        public ILineItem BuildEntity(OrderItemDto dto)
        {
            var orderLineItem = new LineItem(dto.ContainerId, dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Amount)
            {
                Id = dto.Id,
                ExtendedData = string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData),
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            orderLineItem.ResetDirtyProperties();

            return orderLineItem;
        }

        public OrderItemDto BuildDto(ILineItem entity)
        {
            return new OrderItemDto()
            {
                Id = entity.Id,
                ContainerId = entity.ContainerId,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Amount = entity.Amount,
                ExtendedData = entity.ExtendedData.Serialize(),
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }


    }
}