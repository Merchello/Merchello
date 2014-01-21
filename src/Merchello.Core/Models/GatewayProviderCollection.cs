using System;
using System.Collections.Specialized;
using System.Threading;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    public class GatewayProviderCollection : NotifiyCollectionBase<Guid, IGatewayProvider>
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
                var key = GetKeyForItem(item);
                if (Guid.Empty != key)
                {
                    var exists = Contains(item.Key);
                    if (exists)
                    {
                        return;
                    }
                }

                base.Add(item);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
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