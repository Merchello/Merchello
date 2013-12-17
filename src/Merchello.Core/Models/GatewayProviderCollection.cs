using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a Gateway provider collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class ShipMethodCollection<T> : KeyedCollection<string, IShipMethod>
        where T : IShipMethod
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override string GetKeyForItem(IShipMethod item)
        {
            return item.ServiceCode;
        }

        internal new void Add(IShipMethod item)
        {
            using (new WriteLock(_addLocker))
            {

                if (Contains(item.ServiceCode)) return;

                base.Add(item);

            }
        }

        public int IndexOfKey(string serviceCode)
        {
            for (var i = 0; i < Count; i++)
            {
                if (GetKeyForItem(this[i]) == serviceCode)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}