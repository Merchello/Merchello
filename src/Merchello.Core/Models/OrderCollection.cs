namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;

    using Umbraco.Core;

    /// <summary>
    /// The order collection.
    /// </summary>
    public class OrderCollection : NotifiyCollectionBase<Guid, IOrder>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override Guid GetKeyForItem(IOrder item)
        {
            return item.Key;
        }


        internal new void Add(IOrder item)
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

        public bool Equals(OrderCollection compare)
        {
            return Count == compare.Count && compare.All(item => Contains(item.Key));
        }
    }

}