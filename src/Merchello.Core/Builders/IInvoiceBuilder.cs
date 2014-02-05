using Merchello.Core.Checkout;
using Merchello.Core.Models;

namespace Merchello.Core.Builders
{
    public interface IInvoiceBuilder : IBuilder
    {
        IInvoice BuildInvoice();
    }
}