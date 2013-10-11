using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemCacheItemFactory : IEntityFactory<ILineItem, CustomerItemCacheItemDto>
    {
        public ILineItem BuildEntity(CustomerItemCacheItemDto dto)
        {
            var lineItem = new OrderLineItem(dto.ItemCacheId, dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Amount)
            {
                Id = dto.Id,
                ExtendedData = string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData),
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }

        public CustomerItemCacheItemDto BuildDto(ILineItem entity)
        {
            var dto = new CustomerItemCacheItemDto()
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

            return dto;
        }
    }
}
