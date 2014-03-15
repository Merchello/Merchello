using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockInvoiceDataMaker : MockDataMakerBase
    {
        public static IInvoice InvoiceForInserting(IAddress billTo, decimal total)
        {
            var status = new InvoiceStatus()
                {
                    Key = Constants.DefaultKeys.InvoiceStatus.Unpaid,
                    Active = true,
                    Alias = "unpaid",
                    Name = "Unpaid",
                    SortOrder = 0
                };
            var invoice = new Invoice(status);
            invoice.SetBillingAddress(billTo);
            invoice.Total = total;

            return invoice;
        }
    }
}