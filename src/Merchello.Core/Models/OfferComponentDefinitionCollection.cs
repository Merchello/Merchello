namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Threading;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Threading;

    /// <summary>
    /// A collection of <see cref="OfferComponentConfiguration"/>s.
    /// </summary>
    public class OfferComponentDefinitionCollection : NotifiyCollectionBase<Guid, OfferComponentDefinition>
    {
        /// <summary>
        /// The add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Returns the index of the key in the collection or -1 if not found
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The index of the key in the collection or -1 if not found.
        /// </returns>
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

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(OfferComponentDefinition item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!Guid.Empty.Equals(key))
                {
                    var exists = Contains(GetKeyForItem(item));
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

        /// <summary>
        /// Returns the ComponentKey for the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        protected override Guid GetKeyForItem(OfferComponentDefinition item)
        {
            return item.ComponentKey;
        }
    }
}