using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading;
using Merchello.Core.Gateways.Shipping;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Gateway provider collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class GatewayShipMethodCollection<T> : KeyedCollection<Guid, IGatewayShipMethod>
        where T : IGatewayShipMethod
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override Guid GetKeyForItem(IGatewayShipMethod item)
        {
            return item.ShipMethod.Key;
        }

        internal new void Add(IGatewayShipMethod item)
        {
            using (new WriteLock(_addLocker))
            {

                if (Contains(GetKeyForItem(item))) return;

                base.Add(item);

            }
        }

        public int IndexOfKey(Guid shipMethodKey)
        {
            for (var i = 0; i < Count; i++)
            {
                if (GetKeyForItem(this[i]) == shipMethodKey)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}