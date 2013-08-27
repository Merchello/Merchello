using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.Data
{
    public class InvoiceData
    {
        public static IInvoiceStatus InvoiceStatusUnpaidMock()
        {
            return new InvoiceStatus()
            {
                Id = 1,
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
                Id = 3,
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
