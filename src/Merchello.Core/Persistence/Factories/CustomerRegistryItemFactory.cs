using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CustomerRegistryItemFactory : IEntityFactory<IPurchaseLineItem, CustomerRegistryItemDto>
    {
        public IPurchaseLineItem BuildEntity(CustomerRegistryItemDto dto)
        {
            var purchaseLineItem = new PurchaseLineItemContainer(dto.CustomerRegistryId)
            {
                Id = dto.Id,
                ParentId = dto.ParentId,
                LineItemTfKey = dto.LineItemTfKey,
                Sku = dto.Sku,
                Name = dto.Name,
                BaseQuantity = dto.BaseQuantity,
                UnitOfMeasureMultiplier = dto.UnitOfMeasureMultiplier,
                Amount = dto.Amount,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            purchaseLineItem.ResetDirtyProperties();

            return purchaseLineItem;
        }

        public CustomerRegistryItemDto BuildDto(IPurchaseLineItem entity)
        {
            var dto = new CustomerRegistryItemDto()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                CustomerRegistryId = entity.ContainerId,
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
