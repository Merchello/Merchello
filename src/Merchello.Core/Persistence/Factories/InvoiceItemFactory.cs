using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InvoiceItemFactory : ILineItemFactory<InvoiceItemDto>
    {
        public ILineItem BuildEntity(InvoiceItemDto dto)
        {
            var invoiceItem = new LineItem(dto.ContainerId, dto.LineItemTfKey, dto.Name, )
            {
                Id = dto.Id,                
                Sku = dto.Sku,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Amount = dto.Amount,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            invoiceItem.ResetDirtyProperties();

            return invoiceItem;
        }

        public InvoiceItemDto BuildDto(ILineItem entity)
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
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }

        public InvoiceItemDto BuildDto(IInvoiceItem entity)
        {
           
        }

        ILineItem IEntityFactory<ILineItem, InvoiceItemDto>.BuildEntity(InvoiceItemDto dto)
        {
            throw new System.NotImplementedException();
        }
    }
}
