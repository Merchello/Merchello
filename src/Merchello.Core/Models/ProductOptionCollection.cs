namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    using Merchello.Core.Threading;

    /// <summary>
    /// Defines a product option collection
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductOptionCollection : NotifiyCollectionBase<Guid, IProductOption>
    {
        /// <summary>
        /// The _add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Overrides the Remove method.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public new bool Remove(IProductOption item)
        {
            if (Guid.Empty.Equals(item.Key) || !Contains(item.Key)) return false;

            this.RemoveItem(item.Key);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(string name)
        {
            return this.Any(x => x.Name == name);
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

        /// <summary>
        /// Adds a new option to the collection.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(IProductOption item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (Guid.Empty != key)
                {
                    var exists = Contains(item.Key);
                    if (exists)
                    {
                        this[key].SortOrder = item.SortOrder;
                        return;
                    }
                }

                // set the sort order to the next highest
                item.SortOrder = this.Any() ? this.Max(x => x.SortOrder) + 1 : 1;
                base.Add(item);
                
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        /// <summary>
        /// The get key for item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        protected override Guid GetKeyForItem(IProductOption item)
        {
            return item.Key;
        }

    }
}