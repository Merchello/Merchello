namespace Merchello.Core.Models.DetachedContent
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The detached content collection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of detached content
    /// </typeparam>
    /// <remarks>
    /// This collection assumes that all content will be for the same entity and thus uses
    /// the culture name as the key.
    /// </remarks>
    public class DetachedContentCollection<T> : NotifiyCollectionBase<string, T>
        where T : IDetachedContent
    {
        /// <summary>
        /// The add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();


        /// <summary>
        /// Gets a value indicating whether or not the current collection is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Count == 0; }
        }
       
        /// <summary>
        /// Gets the index of the key in the current collection.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The index of the key in the collection or -1 if not found.
        /// </returns>
        public override int IndexOfKey(string key)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].CultureName == key)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether this collection contains a <see cref="T"/> whose culture name matches the culture name.
        /// </summary>
        /// <param name="cultureName">The culture name</param>
        /// <returns><c>true</c> if the collection contains the specified culture name; otherwise, <c>false</c>.</returns>
        public new bool Contains(string cultureName)
        {
            return this.Any(x => x.CultureName == cultureName);
        }

        /// <summary>
        /// Adds a collection of <see cref="T"/> to the collection.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void Add(IEnumerable<T> items)
        {
            items.ForEach(this.Add);
        }

        /// <summary>
        /// Allows visitor to visit each item in the collection
        /// </summary>
        /// <param name="visitor">A <see cref="IVisitor{T}"/></param>
        public virtual void Accept(IVisitor<T> visitor)
        {
            foreach (var item in this)
            {
                visitor.Visit(item);
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(T item)
        {
            using (new WriteLock(this._addLocker))
            {
                var key = this.GetKeyForItem(item);
                if (key != null)
                {
                    var exists = this.Contains(key);
                    if (exists)
                    {
                        return;
                    }
                }

                base.Add(item);

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        /// <summary>
        /// Gets the property that is used as the key for this collection.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The key for the collection.
        /// </returns>
        protected override string GetKeyForItem(T item)
        {
            return item.CultureName;
        }
    }
}