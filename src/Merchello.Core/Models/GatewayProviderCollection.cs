namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Threading;

    using Umbraco.Core;

    /// <summary>
    /// The gateway provider collection.
    /// </summary>
    public class GatewayProviderCollection : NotifiyCollectionBase<Guid, IGatewayProviderSettings>
    {
        /// <summary>
        /// The add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets the index of a key in the collection.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(IGatewayProviderSettings item)
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

        /// <summary>
        /// Gets the key for an item in the collection.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        protected override Guid GetKeyForItem(IGatewayProviderSettings item)
        {
            return item.Key;
        }
    }
}