using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product option collection
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductOptionCollection : KeyedCollection<string, IProductOption>, INotifyCollectionChanged 
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override string GetKeyForItem(IProductOption item)
        {
            return item.Name;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;



        protected override void SetItem(int index, IProductOption item)
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

        protected override void InsertItem(int index, IProductOption item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        internal new void Add(IProductOption item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!string.IsNullOrEmpty(key))
                {
                    var exists = Contains(item.Name);
                    if (exists)
                    {
                        this[key].SortOrder = item.SortOrder;
                        return;
                    }
                }

                // set the sort order to the next highest
                item.SortOrder = this.Any() ? this.Max(x => x.SortOrder) + 1 : 1;
                base.Add(item);
                
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public void RemoveItem(string name)
        {
            var key = IndexOfKey(name);
            //Only removes an item if the key was found
            if (key != -1)
                RemoveItem(key);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }

        public new bool Contains(string name)
        {
            return this.Any(x => x.Name == name);
        }

        public int IndexOfKey(string key)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == key)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}