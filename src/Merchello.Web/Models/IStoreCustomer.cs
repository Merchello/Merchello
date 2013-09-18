using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models
{
    public interface IStoreCustomer
    {
        ICustomer Customer { get; }

        IEnumerable<IInvoice> Invoices { get; }

        IEnumerable<IPayment> Payments { get; }

        IEnumerable<IAddress> Addresses { get; }
    }
}