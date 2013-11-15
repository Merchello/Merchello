using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product variant inventory collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class WarehouseInventoryCollection : NotifiyCollectionBase<string, IWarehouseInventory>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override string GetKeyForItem(IWarehouseInventory item)
        {
            return MakeKeyForItem(item);
        }


        internal new void Add(IWarehouseInventory item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!string.IsNullOrEmpty(key))
                {
                    var exists = Contains(MakeKeyForItem(item));
                    if (exists)
                    {
                        return;
                    }
                }

                // set the sort order to the next highest
                base.Add(item);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public new bool Contains(string key)
        {
            return this.Any(x => MakeKeyForItem(x) == key);
        }

        public bool Contains(Guid warehouseKey)
        {
            return this.Any(x => x.WarehouseKey == warehouseKey);
        }

        public override int IndexOfKey(string key)
        {
            for (var i = 0; i < Count; i++)
            {
                if (GetKeyForItem(this[i]) == key)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string MakeKeyForItem(IWarehouseInventory item)
        {
            return string.Format("{0}-{1}", item.ProductVariantKey, item.WarehouseKey);
        }
        
    }
}