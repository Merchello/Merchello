using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemRegisterItemFactory : IEntityFactory<IOrderLineItem, CustomerItemRegisterItemDto>
    {
        public IOrderLineItem BuildEntity(CustomerItemRegisterItemDto dto)
        {
            var lineItem = new OrderLineItem(dto.CustomerItemRegisterId, dto.LineItemTfKey)
            {
                Id = dto.Id,
                ParentId = dto.ParentId,
                Sku = dto.Sku,
                Name = dto.Name,
                BaseQuantity = dto.BaseQuantity,
                UnitOfMeasureMultiplier = dto.UnitOfMeasureMultiplier,
                Amount = dto.Amount,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }

        public CustomerItemRegisterItemDto BuildDto(IOrderLineItem entity)
        {
            var dto = new CustomerItemRegisterItemDto()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                CustomerItemRegisterId = entity.ContainerId,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                BaseQuantity = entity.BaseQuantity,
                UnitOfMeasureMultiplier = entity.UnitOfMeasureMultiplier,
                Amount = entity.Amount,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
