using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a ShipMethod collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class ShipMethodCollection : NotifiyCollectionBase<Guid, IShipMethod>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override Guid GetKeyForItem(IShipMethod item)
        {
            return item.Key;
        }

        internal new void Add(IShipMethod item)
        {
            using (new WriteLock(_addLocker))
            {
             
                if (Contains(item.Key)) return;

                base.Add(item);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }
    
        public override int IndexOfKey(Guid key)
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

    }
}