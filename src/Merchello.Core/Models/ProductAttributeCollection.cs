namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    using Umbraco.Core;

    /// <summary>
    /// Defines a product attribute collection
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    [KnownType(typeof(ProductAttribute))]
    public class ProductAttributeCollection : NotifiyCollectionBase<Guid, IProductAttribute>
    {
        /// <summary>
        /// The thread locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// The equals comparison.
        /// </summary>
        /// <param name="compare">
        /// The compare.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(ProductAttributeCollection compare)
        {
            return Count == compare.Count && compare.All(item => Contains(item.Key));
        }

        /// <summary>
        /// Gets the index of a given key.
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
        /// Performs the contains operation based off the option choice SKU.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
        }

        /// <summary>
        /// Overrides the Remove method.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public new bool Remove(IProductAttribute item)
        {
            if (Guid.Empty.Equals(item.Key) || !Contains(item.Key)) return false;

            this.RemoveItem(item.Key);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;            
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal new void Add(IProductAttribute item)
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


                if (item.SortOrder == 0)
                {
                    // set the sort order to the next highest
                    item.SortOrder = this.Any() ? this.Max(x => x.SortOrder) + 1 : 1;
                }
                

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
        protected override Guid GetKeyForItem(IProductAttribute item)
        {
            return item.Key;
        }

    }
}