namespace Merchello.Core.Models
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// Defines an abstract class for key collections with notification events
    /// </summary>
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <typeparam name="TItem">The type of the item</typeparam>
    public abstract class NotifiyCollectionBase<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        /// <summary>
        /// An event handler to be triggered when the collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void RemoveItem(TKey key)
        {
            var index = IndexOfKey(key);

            //// Only removes an item if the key was found
            if (index != -1)
                RemoveItem(index);
        }

        /// <summary>
        /// Gets the internal collection item index for the key from the collection.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public abstract int IndexOfKey(TKey key);

        /// <summary>
        /// Gets the key for the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="TKey"/>.
        /// </returns>
        protected abstract override TKey GetKeyForItem(TItem item);

        /// <summary>
        /// Sets an item in the collection.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        protected override void RemoveItem(int index)
        {
            var removed = this[index];
            base.RemoveItem(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }

        /// <summary>
        /// Inserts an item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Clears all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    
    }
}