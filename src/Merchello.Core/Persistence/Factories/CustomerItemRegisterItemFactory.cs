using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemRegisterItemFactory : IEntityFactory<ILineItem, CustomerItemCacheItemDto>
    {
        public ILineItem BuildEntity(CustomerItemCacheItemDto dto)
        {
            var lineItem = new OrderLineItem(dto.ItemCacheId, dto.LineItemTfKey)
            {
                Id = dto.Id,
                Sku = dto.Sku,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Amount = dto.Amount,
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
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
