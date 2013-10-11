using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Umbraco.Core;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a Collection of <see cref="LineItemBase"/> objects
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class LineItemCollection : NotifiyCollectionBase<string, LineItemBase>
    {
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();
        internal Action OnAdd;
        internal Func<LineItemBase, bool> ValidateAdd { get; set; }

        internal new void Add(LineItemBase item)
        {
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (key != null)
                {
                    var exists = this.Contains(key);
                    if (!exists) return;
                    this[key].Quantity += item.Quantity;
                    return;
                }
                base.Add(item);

                OnAdd.IfNotNull(x => x.Invoke());

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        protected override string GetKeyForItem(LineItemBase item)
        {
            return item.Sku;
        }

        public override int IndexOfKey(string sku)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Sku == sku)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determines whether this collection contains a <see cref="ILineItem"/> whose sku matches the specified sku.
        /// </summary>
        /// <param name="sku">Sku of the line item.</param>
        /// <returns><c>true</c> if the collection contains the specified sku; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
        }

        /// <summary>
        /// Allows visitor to visit each item in the collection
        /// </summary>
        /// <param name="visitor"><see cref="ILineItemVisitor"/></param>
        public virtual void Accept(ILineItemVisitor visitor)
        {
            foreach (var item in this)
            {
                visitor.Visit(item);
            }
        }
    }
}