using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Builders
{
    public interface IInvoiceBuilder : IBuilder
    {
        Attempt<IInvoice> BuildInvoice();
    }
}