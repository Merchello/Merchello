namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Threading;

    /// <summary>
    /// Represents a Collection of <see cref="T"/> objects
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    public class LineItemCollection : NotifiyCollectionBase<string, ILineItem>
    {
        /// <summary>
        /// The add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemCollection"/> class.
        /// </summary>
        public LineItemCollection()
        {            
        }

        /// <summary>
        /// Event handler for AddingItem.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="arg">
        /// The event argument.
        /// </param>
        public delegate void AddItemEventHandler(object sender, AddItemEventArgs arg);

        /// <summary>
        /// The adding tier.
        /// </summary>
        public event AddItemEventHandler AddingItem;

        /// <summary>
        /// Gets a value indicating whether or not the current collection is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Count == 0; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemCollection"/> class.
        /// </summary>
        /// <param name="validationCallback">
        /// The validation callback.
        /// </param>
        public LineItemCollection(Func<ILineItem, bool> validationCallback)
        {
            this.ValidateAdd = validationCallback;
        }


        /// <summary>
        /// Gets or sets the validate add.
        /// </summary>
        internal Func<ILineItem, bool> ValidateAdd { get; set; }


        /// <summary>
        /// Gets the index of the key.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int IndexOfKey(string sku)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].Sku == sku)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether this collection contains a <see cref="T"/> whose SKU matches the specified SKU.
        /// </summary>
        /// <param name="sku">SKU of the line item.</param>
        /// <returns><c>true</c> if the collection contains the specified SKU; otherwise, <c>false</c>.</returns>
        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
        }

        /// <summary>
        /// Adds a collection of <see cref="ILineItem"/> to the collection.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void Add(IEnumerable<ILineItem> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Allows visitor to visit each item in the collection
        /// </summary>
        /// <param name="visitor">A <see cref="ILineItemVisitor"/></param>
        public virtual void Accept(ILineItemVisitor visitor)
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
        internal new void Add(ILineItem item)
        {
            using (new WriteLock(this._addLocker))
            {
                var key = this.GetKeyForItem(item);
                if (key != null)
                {
                    var exists = this.Contains(key);
                    if (exists)
                    {
                        this[key].Quantity += item.Quantity;
                        return;
                    }
                }

                if (this.ValidateAdd != null) if (!this.ValidateAdd(item)) return;

                this.AddingItem?.Invoke(this, new AddItemEventArgs(item));

                base.Add(item);

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        /// <summary>
        /// Gets the key for the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetKeyForItem(ILineItem item)
        {
            return item.Sku;
        }
    }
}