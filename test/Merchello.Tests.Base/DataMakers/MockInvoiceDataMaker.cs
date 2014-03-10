using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    public class MockInvoiceDataMaker : MockDataMakerBase
    {
        public static IInvoice InvoiceForInserting(IAddress billTo, decimal total)
        {
            var invoice = new Invoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
            invoice.SetBillingAddress(billTo);
            invoice.Total = total;

            return invoice;
        }
    }
}