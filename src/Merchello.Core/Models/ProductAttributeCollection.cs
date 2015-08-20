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
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override Guid GetKeyForItem(IProductAttribute item)
        {
            return item.Key;
        }

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

                // set the sort order to the next highest
                //item.SortOrder = this.Any() ? this.Max(x => x.SortOrder) + 1 : 1;
                base.Add(item);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        public bool Equals(ProductAttributeCollection compare)
        {
            return Count == compare.Count && compare.All(item => Contains(item.Key));
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


        public bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
        }

        //public bool Contains(Guid key)
        //{
        //    return this.Any(x => x.Key == key);
        //}

        

    }
}