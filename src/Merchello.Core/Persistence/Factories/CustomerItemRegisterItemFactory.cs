using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerItemRegisterItemFactory : IEntityFactory<ILineItem, CustomerItemRegisterItemDto>
    {
        public ILineItem BuildEntity(CustomerItemRegisterItemDto dto)
        {
            var lineItem = new OrderLineItem(dto.CustomerItemRegisterId, dto.LineItemTfKey)
            {
                Id = dto.Id,
                ParentId = dto.ParentId,
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

        public CustomerItemRegisterItemDto BuildDto(ILineItem entity)
        {
            var dto = new CustomerItemRegisterItemDto()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                CustomerItemRegisterId = entity.ContainerId,
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
