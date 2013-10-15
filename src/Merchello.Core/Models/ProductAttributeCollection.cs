using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product attribute collection
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductAttributeCollection : NotifiyCollectionBase<string, IProductAttribute>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override string GetKeyForItem(IProductAttribute item)
        {
            return item.Sku;
        }

        internal new void Add(IProductAttribute item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (!string.IsNullOrEmpty(key))
                {
                    var exists = Contains(item.Sku);
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

        public bool Equals(ProductAttributeCollection compare)
        {
            return Count == compare.Count && compare.All(item => Contains(item.Id));
        }

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


        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku  == sku);
        }

        public bool Contains(int id)
        {
            return this.Any(x => x.Id == id);
        }

        

    }
}