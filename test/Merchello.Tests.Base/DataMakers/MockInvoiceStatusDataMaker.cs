using System;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockInvoiceStatusDataMaker : MockDataMakerBase
    {
        public static IInvoiceStatus InvoiceStatusUnpaidMock()
        {
            return new InvoiceStatus()
            {
                Key = Guid.NewGuid(),
                Alias = "unpaid",
                Name = "Unpaid",
                Active = true,
                Reportable = true,
                SortOrder = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now

            };
        }

        public static IInvoiceStatus InvoiceStatusCompletedMock()
        {
            return new InvoiceStatus()
            {
                Key = Guid.NewGuid(),
                Alias = "completed",
                Name = "Completed",
                Active = true,
                Reportable = true,
                SortOrder = 3,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now

            };
        }
    }
}
