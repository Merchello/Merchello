using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Threading;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Gateway provider collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class GatewayProviderCollection<T> : NotifiyCollectionBase<Guid, IGatewayProvider>
        where T : IGatewayProvider
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override Guid GetKeyForItem(IGatewayProvider item)
        {
            return item.Key;
        }

        internal new void Add(IGatewayProvider item)
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