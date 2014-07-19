using System.Diagnostics.CodeAnalysis;

namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net.Configuration;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;
    using Umbraco.Core;

    /// <summary>
    /// Represents a Collection of <see cref="T"/> objects
    /// </summary>
    [Serializable]
    [CollectionDataContract(IsReference = true)]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")] 
    public class LineItemCollection : NotifiyCollectionBase<string, ILineItem>
    {
        #region Fields

        /// <summary>
        /// The add locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _addLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// The on add.
        /// </summary>
        private Action OnAdd;

#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemCollection"/> class.
        /// </summary>
        public LineItemCollection()
        {            
        }

        /// <summary>
        /// True/false indicating whether or not the current collection is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemCollection"/> class.
        /// </summary>
        /// <param name="validationCallback">
        /// The validation callback.
        /// </param>
        public LineItemCollection(Func<ILineItem, bool> validationCallback)
        {
            ValidateAdd = validationCallback;
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
            items.ForEach(Add);
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
            using (new WriteLock(_addLocker))
            {
                var key = GetKeyForItem(item);
                if (key != null)
                {
                    var exists = Contains(key);
                    if (exists)
                    {
                        this[key].Quantity += item.Quantity;
                        return;
                    }
                }

                if (ValidateAdd != null) if (!ValidateAdd(item)) return;

                base.Add(item);

                OnAdd.IfNotNull(x => x.Invoke());

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
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