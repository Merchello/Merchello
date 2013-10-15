using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class LineItemFactory 
    {       
        public ItemCacheItemDto BuildDto(IItemCacheLineItem entity)
        {
            var dto = new ItemCacheItemDto()
            {
                Id = entity.Id,
                ContainerId = entity.ContainerId,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Amount = entity.Amount,
                ExtendedData = entity.ExtendedData.Serialize(),
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }

        public InvoiceItemDto BuildDto(IInvoiceLineItem entity)
        {
            var dto = new InvoiceItemDto()
            {
                Id = entity.Id,
                ContainerId = entity.ContainerId,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Amount = entity.Amount,
                ExtendedData = entity.ExtendedData.Serialize(),
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }

        public OrderItemDto BuildDto(IOrderLineItem entity)
        {
            var dto = new OrderItemDto()
            {
                Id = entity.Id,
                ContainerId = entity.ContainerId,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Amount = entity.Amount,
                ExtendedData = entity.ExtendedData.Serialize(),
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }

        public ItemCacheLineItem BuildEntity(ItemCacheItemDto dto)
        {
             var lineItem = new ItemCacheLineItem(dto.ContainerId, dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Amount,
                 string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData))
            {
                Id = dto.Id,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }

        public InvoiceLineItem BuildEntity(InvoiceItemDto dto)
        {
          var lineItem = new InvoiceLineItem(dto.ContainerId, dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Amount,
              string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData))
            {
                Id = dto.Id,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }

        public OrderLineItem BuildEntity(OrderItemDto dto)
        {
            var lineItem = new OrderLineItem(dto.ContainerId, dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Amount,
                string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData))
            {
                Id = dto.Id,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }
    }
}
