using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Merchello.Core.Models
{
    public class InvoiceItemItemization : IInvoiceItemItemization, INotifyCollectionChanged
    {

        private readonly IList<IInvoiceItem> _items; 

        public InvoiceItemItemization(IList<IInvoiceItem> items)
        {
            _items = items;
        }

        #region Implementation of IInvoiceItemItemization
        
        public IEnumerable<IInvoiceItem> InvoiceItems
        {
            get { return _items; }
        }

        /// <summary>
        /// Adds an invoice item to the collection
        /// </summary>
        /// <param name="item">The <see cref="IInvoiceItem"/> to be added to the collection</param>
        public void InsertItem(IInvoiceItem item)
        {
            _items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Removes an invoice item from the collection
        /// </summary>
        /// <param name="item">Removes an <see cref="IInvoiceItem"/> from the collection</param>
        public void RemoveItem(IInvoiceItem item)
        {
            var index = IndexOfItem(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
        }

        /// <summary>
        /// Clears the collection of all invoice items
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IInvoiceItem item)
        {
            return _items.Any(x => x.Id == item.Id);
        }


        public int IndexOfItem(IInvoiceItem item)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id == item.Id)
                {
                    return i;
                }
            }
            return -1;
        }
          
        public decimal Total()
        {
            return _items.Sum(x => x.Total());
        }
        #endregion




        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }

        }

    }
}
