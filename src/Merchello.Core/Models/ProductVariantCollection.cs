namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    using Merchello.Core.Threading;

    /// <summary>
    /// Represents a collection of <see cref="IProductVariant"/>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductVariantCollection : NotifiyCollectionBase<string, IProductVariant>
    {
        /// <summary>
        /// The thread locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Returns a value indicating whether or not the collection contains an item with a SKU passed as the parameter.
        /// </summary>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
        }

        /// <summary>
        /// Gets the collection index of the key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int IndexOfKey(string key)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Sku == key)
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
        internal new void Add(IProductVariant item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!string.IsNullOrEmpty(key))
                {
                    var exists = Contains(item.Name);
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
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetKeyForItem(IProductVariant item)
        {
            return item.Sku;
        }
    }
}