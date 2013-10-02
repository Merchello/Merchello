using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal partial class InvoiceItemFactory : IEntityFactory<IInvoiceItem, InvoiceItemDto>
    {
        public IInvoiceItem BuildEntity(InvoiceItemDto dto)
        {
            var invoiceItem = new InvoiceItem(dto.InvoiceId, dto.LineItemTfKey, dto.ParentId)
            {
                Id = dto.Id,                
                Sku = dto.Sku,
                Name = dto.Name,
                Quantity = dto.Quantity,
                UnitOfMeasureMultiplier = dto.UnitOfMeasureMultiplier,
                Amount = dto.Amount,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            invoiceItem.ResetDirtyProperties();

            return invoiceItem;
        }

        public InvoiceItemDto BuildDto(IInvoiceItem entity)
        {
            var dto = new InvoiceItemDto()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                InvoiceId = entity.InvoiceId,
                LineItemTfKey = entity.InvoiceItemTfKey,
                Sku = entity.Sku,
                Name = entity.Name,
                Quantity = entity.Quantity,
                UnitOfMeasureMultiplier = entity.UnitOfMeasureMultiplier,
                Amount = entity.Amount,
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }

    }
}
