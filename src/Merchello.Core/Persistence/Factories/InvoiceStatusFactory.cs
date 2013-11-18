using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InvoiceStatusFactory : IEntityFactory<IInvoiceStatus, InvoiceStatusDto>
    {
        public IInvoiceStatus BuildEntity(InvoiceStatusDto dto)
        {
            var invoiceStatus = new InvoiceStatus()
            {
                Key = dto.Key,
                Name = dto.Name,
                Alias = dto.Alias,
                Reportable = dto.Reportable,
                Active = dto.Active,                
                SortOrder = dto.SortOrder,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            invoiceStatus.ResetDirtyProperties();

            return invoiceStatus;
        }

        public InvoiceStatusDto BuildDto(IInvoiceStatus entity)
        {
            var dto = new InvoiceStatusDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                Alias = entity.Alias,
                Reportable = entity.Reportable,
                Active = entity.Active,
                SortOrder = entity.SortOrder,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}
