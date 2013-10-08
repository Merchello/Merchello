using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a collection of <see cref="IProductVariant"/>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductVariantCollection : NotifiyCollectionBase<string, IProductVariant>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        protected override string GetKeyForItem(IProductVariant item)
        {
            return item.Sku;
        }

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

        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
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
    }
}