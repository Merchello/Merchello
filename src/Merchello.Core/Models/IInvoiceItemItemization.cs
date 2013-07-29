using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Marker interface for InvoiceItemItemization
    /// </summary>
    public interface IInvoiceItemItemization : IItemization
    {
        IEnumerable<IInvoiceItemBase> InvoiceItems { get; };
    }
}
