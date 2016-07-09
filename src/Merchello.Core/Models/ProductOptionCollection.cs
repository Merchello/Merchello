using System;
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
    public class ProductOptionCollection : NotifiyCollectionBase<Guid, IProductOption>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override Guid GetKeyForItem(IProductOption item)
        {
            return item.Key;
        }

        // TODO 2.2.0 should not need to CAST to ProductOption 
        // This is a quick fix in the 2.2.0 refactoring and needs to be addressed before release
        internal new void Add(IProductOption item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (Guid.Empty != key)
                {
                    var exists = Contains(item.Key);
                    if (exists)
                    {
                        ((ProductOption)this[key]).SortOrder = item.SortOrder;
                        return;
                    }
                }

                // set the sort order to the next highest
                ((ProductOption)item).SortOrder = this.Any() ? this.Max(x => x.SortOrder) + 1 : 1;
                base.Add(item);
                
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

       
        public bool Contains(string name)
        {
            return this.Any(x => x.Name == name);
        }

        public override int IndexOfKey(Guid key)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Key == key)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}