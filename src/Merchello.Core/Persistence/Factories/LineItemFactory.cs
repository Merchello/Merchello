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
                Key = entity.Key,
                ContainerKey = entity.ContainerKey,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Price = entity.Price,
                ExtendedData = entity.ExtendedData.SerializeToXml(),
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
                Key = entity.Key,
                ContainerKey = entity.ContainerKey,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Price = entity.Price,
                ExtendedData = entity.ExtendedData.SerializeToXml(),
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
                Key = entity.Key,
                ContainerKey = entity.ContainerKey,
                LineItemTfKey = entity.LineItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                Price = entity.Price,
                ExtendedData = entity.ExtendedData.SerializeToXml(),
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }

        public ItemCacheLineItem BuildEntity(ItemCacheItemDto dto)
        {
             var lineItem = new ItemCacheLineItem(dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Price,
                 string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData))
            {
                Key = dto.Key,
                ContainerKey = dto.ContainerKey,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }

        public InvoiceLineItem BuildEntity(InvoiceItemDto dto)
        {
          var lineItem = new InvoiceLineItem(dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Price,
              string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData))
            {
                Key = dto.Key,
                ContainerKey = dto.ContainerKey,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }

        public OrderLineItem BuildEntity(OrderItemDto dto)
        {
            var lineItem = new OrderLineItem(dto.LineItemTfKey, dto.Name, dto.Sku, dto.Quantity, dto.Price,
                string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData))
            {
                Key = dto.Key,
                ContainerKey = dto.ContainerKey,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            lineItem.ResetDirtyProperties();

            return lineItem;
        }
    }
}
