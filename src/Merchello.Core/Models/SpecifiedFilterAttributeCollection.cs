namespace Merchello.Core.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The specified filter attribute collection.
    /// </summary>
    public class SpecifiedFilterAttributeCollection : KeyedCollection<Guid, IEntityCollection>
    {
        /// <summary>
        /// The _add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// The index of key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int IndexOfKey(Guid key)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].Key == key)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes the <see cref="IEntityCollection"/> from the Specification Collection.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// Returns a value indicating whether or not the item was removed from the collection.
        /// </returns>
        public new bool Remove(IEntityCollection item)
        {
            if (Guid.Empty.Equals(item.Key) || !Contains(item.Key)) return false;

            this.RemoveItem(IndexOfKey(item.Key));
            return true;
        }

        /// <summary>
        /// Adds a entity collection to the collection.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(IEntityCollection item)
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
            }
        }

        /// <summary>
        /// Gets the key for the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        protected override Guid GetKeyForItem(IEntityCollection item)
        {
            return item.Key;
        }
    }
}