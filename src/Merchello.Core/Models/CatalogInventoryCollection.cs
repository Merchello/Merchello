namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    using Umbraco.Core;

    /// <summary>
    /// Defines a product variant inventory collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    [KnownType(typeof(CatalogInventory))]
    public class CatalogInventoryCollection : NotifiyCollectionBase<string, ICatalogInventory>
    {
        /// <summary>
        /// The add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// The make key for item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MakeKeyForItem(ICatalogInventory item)
        {
            return string.Format("{0}-{1}", item.ProductVariantKey, item.CatalogKey);
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public new bool Contains(string key)
        {
            return this.Any(x => MakeKeyForItem(x) == key);
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(Guid warehouseKey)
        {
            return this.Any(x => x.CatalogKey == warehouseKey);
        }

        /// <summary>
        /// The index of key.
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
        internal new void Add(ICatalogInventory item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!string.IsNullOrEmpty(key))
                {
                    var exists = Contains(MakeKeyForItem(item));
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
        /// The get key for item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetKeyForItem(ICatalogInventory item)
        {
            return MakeKeyForItem(item);
        }
    }
}