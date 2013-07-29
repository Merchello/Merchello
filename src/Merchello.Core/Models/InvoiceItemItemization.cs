using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core.Models
{
    public class InvoiceItemItemization : IInvoiceItemItemization
    {
        private readonly IEnumerable<IInvoiceItemBase> _items; 

        public InvoiceItemItemization(IEnumerable<IInvoiceItemBase> items)
        {
            _items = items;
        }

        #region Implementation of IInvoiceItemItemization
        
        public IEnumerable<IInvoiceItemBase> InvoiceItems
        {
            get { return _items; }
        }

      

        public decimal Total()
        {
            return _items.Sum(x => x.Total());
        }


        #endregion
    }
}
