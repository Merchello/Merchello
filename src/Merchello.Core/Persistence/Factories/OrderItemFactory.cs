using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemCacheLineItemFactory : IEntityFactory<IOrderLineItem, CustomerItemCacheItemDto>
    {
        public IOrderLineItem BuildEntity(CustomerItemCacheItemDto dto)
        {
            var orderLineItem = new OrderLineItem(dto.ItemCacheId, dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Amount)
            {   
                Id = dto.Id,
                ExtendedData = string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData),
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            orderLineItem.ResetDirtyProperties();

            return orderLineItem;
        }

        public CustomerItemCacheItemDto BuildDto(IOrderLineItem entity)
        {
            return new CustomerItemCacheItemDto()
            {
                Id = entity.Id,
                ItemCacheId = entity.ContainerId,
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