using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InvoiceFactory : IEntityFactory<IInvoice, InvoiceDto>
    {
        private readonly LineItemCollection _lineItemCollection;
        private readonly OrderCollection _orderCollection;

        public InvoiceFactory(LineItemCollection lineItemCollection, OrderCollection orderCollection)
        {
            _lineItemCollection = lineItemCollection;
            _orderCollection = orderCollection;
        }

        public IInvoice BuildEntity(InvoiceDto dto)
        {
            var factory = new InvoiceStatusFactory();
            var invoice = new Invoice(factory.BuildEntity(dto.InvoiceStatusDto))
                {
                    Key = dto.Key,
                    CustomerKey = dto.CustomerKey,
                    InvoiceNumberPrefix = dto.InvoiceNumberPrefix,
                    InvoiceNumber = dto.InvoiceNumber,
                    InvoiceDate = dto.InvoiceDate,
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
                    ExamineId = dto.InvoiceIndexDto.Id,
                    Exported = dto.Exported,
                    Archived = dto.Archived,
                    Total = dto.Total,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate,
                    Items = _lineItemCollection,
                    Orders = _orderCollection
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
                    Archived = entity.Archived,
                    Total = entity.Total,
                    InvoiceIndexDto = new InvoiceIndexDto()
                    {
                        Id = ((Invoice)entity).ExamineId,
                        InvoiceKey = entity.Key,
                        UpdateDate = entity.UpdateDate,
                        CreateDate = entity.CreateDate
                    },
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}