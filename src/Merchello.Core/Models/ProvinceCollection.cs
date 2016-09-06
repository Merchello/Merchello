namespace Merchello.Core.Models
{
    using System.Collections.Specialized;
    using System.Threading;

    using Merchello.Core.Threading;

    /// <summary>
    /// Defines a collection of <see cref="IProvince"/>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the province
    /// </typeparam>
    public class ProvinceCollection<T> :  NotifiyCollectionBase<string, T>
        where T : IProvince
    {
        /// <summary>
        /// The thread locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Gets the index of code in the collection.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int IndexOfKey(string code)
        {
            for (var i = 0; i < Count; i++)
            {
                if (GetKeyForItem(this[i]) == code)
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
        internal new void Add(T item)
        {
            using (new WriteLock(_addLocker))
            {
                if (Contains(item.Code)) return;

                base.Add(item);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        /// <summary>
        /// Gets key for item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetKeyForItem(T item)
         {
             return item.Code;
         }
    }
}