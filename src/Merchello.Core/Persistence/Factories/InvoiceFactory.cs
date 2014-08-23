namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The invoice factory.
    /// </summary>
    internal class InvoiceFactory : IEntityFactory<IInvoice, InvoiceDto>
    {
        /// <summary>
        /// The line item collection.
        /// </summary>
        private readonly LineItemCollection _lineItemCollection;

        /// <summary>
        /// The order collection.
        /// </summary>
        private readonly OrderCollection _orderCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceFactory"/> class.
        /// </summary>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        /// <param name="orderCollection">
        /// The order collection.
        /// </param>
        public InvoiceFactory(LineItemCollection lineItemCollection, OrderCollection orderCollection)
        {
            _lineItemCollection = lineItemCollection;
            _orderCollection = orderCollection;
        }

        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IInvoice"/>.
        /// </returns>
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
                    PoNumber = dto.PoNumber,
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

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDto"/>.
        /// </returns>
        public InvoiceDto BuildDto(IInvoice entity)
        {
            return new InvoiceDto()
                {
                    Key = entity.Key,
                    CustomerKey = entity.CustomerKey,
                    InvoiceNumberPrefix = entity.InvoiceNumberPrefix,
                    InvoiceNumber = entity.InvoiceNumber,
                    InvoiceDate = entity.InvoiceDate,
                    PoNumber = entity.PoNumber,
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