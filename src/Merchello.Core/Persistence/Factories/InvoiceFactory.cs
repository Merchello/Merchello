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
                    InvoiceNumberPrefix = dto.InvoiceNumberPrefix,
                    InvoiceNumber = dto.InvoiceNumber,
                    InvoiceDate = dto.InvoiceDate,
                    InvoiceStatusKey = dto.InvoiceStatusKey,
                    VersionKey = dto.VersionKey,
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
                    Total = dto.Total,
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
                    InvoiceNumberPrefix = entity.InvoiceNumberPrefix,
                    InvoiceNumber = entity.InvoiceNumber,
                    InvoiceDate = entity.InvoiceDate,
                    InvoiceStatusKey = entity.InvoiceStatusKey,
                    VersionKey = entity.VersionKey,
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
                    Total = entity.Total,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}