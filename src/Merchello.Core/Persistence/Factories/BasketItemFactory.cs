using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class BasketItemFactory : IEntityFactory<IBasketItem, BasketItemDto>
    {
        public IBasketItem BuildEntity(BasketItemDto dto)
        {
            var basketItem = new BasketItem(dto.BasketId)
            {
                Id = dto.Id,
                ParentId = dto.ParentId,
                InvoiceItemTypeFieldKey = dto.LineItemTfKey,
                Sku = dto.Sku,
                Name = dto.Name,
                BaseQuantity = dto.BaseQuantity,
                UnitOfMeasureMultiplier = dto.UnitOfMeasureMultiplier,
                Amount = dto.Amount,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            basketItem.ResetDirtyProperties();

            return basketItem;
        }

        public BasketItemDto BuildDto(IBasketItem entity)
        {
            var dto = new BasketItemDto()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                BasketId = entity.BasketId,
                LineItemTfKey = entity.InvoiceItemTypeFieldKey,
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
