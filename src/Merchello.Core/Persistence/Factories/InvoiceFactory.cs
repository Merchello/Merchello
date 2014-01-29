using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InvoiceFactory : IEntityFactory<IInvoice, InvoiceDto>
    {
        private readonly LineItemCollection _lineItemCollection;

        public InvoiceFactory(LineItemCollection lineItemCollection)
        {
            _lineItemCollection = lineItemCollection;
        }

        public IInvoice BuildEntity(InvoiceDto dto)
        {
            var invoice = new Invoice(dto.InvoiceStatusKey)
                {
                    Key = dto.Key,
                    CustomerKey = dto.CustomerKey,
                    InvoiceNumber = dto.InvoiceNumber,
                    InvoiceDate = dto.InvoiceDate,
                    BillToName = dto.BillToName,
                    BillToAddress1 = dto.BillToAddress1,
                    BillToAddress2 = dto.BillToAddress2,
                    BillToLocality = dto.BillToLocality,
                    BillToRegion = dto.BillToRegion,
                    BillToPostalCode = dto.BillToPostalCode,
                    BillToCountryCode = dto.BillToCountryCode,
                    BillToEmail = dto.BillToEmail,
                    BillToPhone = dto.BillToPhone,
                    BillToCompany = dto.BillToCompany,
                    Exported = dto.Exported,
                    Paid = dto.Paid,
                    Amount = dto.Amount,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate,
                    Items = _lineItemCollection
                };

            invoice.ResetDirtyProperties();

            return invoice;
        }

        public InvoiceDto BuildDto(IInvoice entity)
        {
            return new InvoiceDto()
                {
                    Key = entity.Key,
                    CustomerKey = entity.CustomerKey,
                    InvoiceNumber = entity.InvoiceNumber,
                    InvoiceDate = entity.InvoiceDate,
                    BillToName = entity.BillToName,
                    BillToAddress1 = entity.BillToAddress1,
                    BillToAddress2 = entity.BillToAddress2,
                    BillToLocality = entity.BillToLocality,
                    BillToRegion = entity.BillToRegion,
                    BillToPostalCode = entity.BillToPostalCode,
                    BillToCountryCode = entity.BillToCountryCode,
                    BillToEmail = entity.BillToEmail,
                    BillToPhone = entity.BillToPhone,
                    BillToCompany = entity.BillToCompany,
                    Exported = entity.Exported,
                    Paid = entity.Paid,
                    Amount = entity.Amount,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}