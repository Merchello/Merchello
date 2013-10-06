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

        internal new void Add(IProductOption item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!string.IsNullOrEmpty(key))
                {
                    var exists = this.Contains(item.Name);
                    if (exists)
                    {
                        return;
                    }
                }
                base.Add(item);
                
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
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