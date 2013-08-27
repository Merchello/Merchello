﻿using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InvoiceFactory : IEntityFactory<IInvoice, InvoiceDto>
    {
        public IInvoice BuildEntity(InvoiceDto dto)
        {

            var invoice = new Invoice(GetCustomer(dto.CustomerDto), GetInvoiceStatus(dto.InvoiceStatusDto), dto.Amount)
            {
                Id = dto.Id,
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
                Shipped = dto.Shipped,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            invoice.ResetDirtyProperties();

            return invoice;
        }

        public InvoiceDto BuildDto(IInvoice entity)
        {
            var dto = new InvoiceDto()
            {
                Id = entity.Id,
                CustomerKey = entity.Customer.Key,
                InvoiceNumber = entity.InvoiceNumber,
                InvoiceDate = entity.InvoiceDate,
                InvoiceStatusId = entity.InvoiceStatus.Id,
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
                Shipped = entity.Shipped,
                Amount = entity.Amount,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }

        private static ICustomer GetCustomer(CustomerDto dto)
        {
            var factory = new CustomerFactory();
            return factory.BuildEntity(dto);
        }

        private static IInvoiceStatus GetInvoiceStatus(InvoiceStatusDto dto)
        {
            var factory = new InvoiceStatusFactory();
            return factory.BuildEntity(dto);
        }
    }
}
