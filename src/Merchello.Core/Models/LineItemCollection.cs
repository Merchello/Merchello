using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a Collection of <see cref="LineItemBase"/> objects
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class LineItemCollection : KeyedCollection<string, LineItemBase>, INotifyCollectionChanged
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();
        internal Action OnAdd;
        internal Func<LineItemBase, bool> ValidateAdd { get; set; }

        internal LineItemCollection()
        {}

        internal LineItemCollection(Func<LineItemBase, bool> validationCallback)
        {
            ValidateAdd = validationCallback;
        }

        public LineItemCollection(IEnumerable<LineItemBase> lineItems)
        {
            Reset(lineItems);
        }

        /// <summary>
        /// Resets the collection to only contain the <see cref="LineItemBase"/> instances referenced in the <paramref name="lineItems"/> parameter, whilst maintaining
        /// any validation delegates such as <see cref="ValidateAdd"/>
        /// </summary>
        /// <param name="lineItems"></param>
        /// <remarks></remarks>
        private void Reset(IEnumerable<LineItemBase> lineItems)
        {
            Clear();
            lineItems.ForEach(Add);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void SetItem(int index, LineItemBase item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            var removed = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal new void Add(LineItemBase item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (key != null)
                {
                    var exists = this.Contains(key);
                    if (exists)
                        return;
                }
                base.Add(item);
                OnAdd.IfNotNull(x => x.Invoke());

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public int IndexOfSku(string sku)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Sku == sku)
                {
                    return i;
                }
            }
            return -1;
        }

        protected override string GetKeyForItem(LineItemBase item)
        {
            return item.Sku;
        }


        ///// <summary>
        ///// Gets the element with the specified ProductActualKey (Guid).
        ///// </summary>
        //internal LineItemBase this[Guid productActualKey]
        //{
        //    get
        //    {
        //        return this.FirstOrDefault(x => x.ProductActualKey == productActualKey);
        //    }
        //}

        protected override void InsertItem(int index, LineItemBase item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Determines whether this collection contains a <see cref="ILineItem"/> whose sku matches the specified sku.
        /// </summary>
        /// <param name="sku">Sku of the line item.</param>
        /// <returns><c>true</c> if the collection contains the specified sku; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
        }


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