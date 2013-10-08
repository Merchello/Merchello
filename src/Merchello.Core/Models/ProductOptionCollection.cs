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
    public class ProductOptionCollection : NotifiyCollectionBase<string, IProductOption>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override string GetKeyForItem(IProductOption item)
        {
            return item.Name;
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

       
        public new bool Contains(string name)
        {
            return this.Any(x => x.Name == name);
        }

        public override int IndexOfKey(string key)
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