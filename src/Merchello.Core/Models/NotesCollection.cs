namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Threading;

    using Umbraco.Core;

    /// <summary>
    /// The notes collection.
    /// </summary>
    public class NotesCollection : NotifiyCollectionBase<Guid, INote>
    {
        /// <summary>
        /// The thread locker for adding to the collection.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets the index of a specific key in the collection
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
        /// Overrides the add method.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(INote item)
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
        /// Gets the key for item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        protected override Guid GetKeyForItem(INote item)
        {
            return item.Key;
        }
    }
}