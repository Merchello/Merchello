using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines an invoice item itemization
    /// </summary>
    public interface IInvoiceItemItemization : IItemization
    {
        /// <summary>
        /// List of InvoiceItems available on this InvoiceItem
        /// </summary>
        IEnumerable<IInvoiceItem> InvoiceItems { get; }

        /// <summary>
        /// Adds an invoice item to the collection
        /// </summary>
        /// <param name="item">The <see cref="IInvoiceItem"/> to be added to the collection</param>
        void InsertItem(IInvoiceItem item);

        /// <summary>
        /// Removes an invoice item from the collection
        /// </summary>
        /// <param name="item">Removes an <see cref="IInvoiceItem"/> from the collection</param>
        void RemoveItem(IInvoiceItem item);

        /// <summary>
        /// Clears the collection of all invoice items
        /// </summary>
        void ClearItems();

    }
}
