using System.Diagnostics.CodeAnalysis;

namespace Merchello.Core.Models
{
    using System;
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
                if(ValidateAdd != null) if(!ValidateAdd(item)) return;
                
                base.Add(item);

                OnAdd.IfNotNull(x => x.Invoke());

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        protected override string GetKeyForItem(ILineItem item)
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
        /// Determines whether this collection contains a <see cref="T"/> whose sku matches the specified sku.
        /// </summary>
        /// <param name="sku">Sku of the line item.</param>
        /// <returns><c>true</c> if the collection contains the specified sku; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public new bool Contains(string sku)
        {
            return this.Any(x => x.Sku == sku);
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