namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a customer base class
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class CustomerBase : Entity, ICustomerBase
    {
        /// <summary>
        /// The _last activity date.
        /// </summary>
        private DateTime _lastActivityDate;
        private Guid _entityKey;
        private ExtendedDataCollection _extendedData;

        protected CustomerBase(bool isAnonymous)
            : this(isAnonymous, new ExtendedDataCollection())
        {
            
        }

        protected CustomerBase(bool isAnonymous, ExtendedDataCollection extendedData)
        {
            IsAnonymous = isAnonymous;
            _extendedData = extendedData;
        }

        private static readonly PropertyInfo LastActivityDateSelector = ExpressionHelper.GetPropertyInfo<CustomerBase, DateTime>(x => x.LastActivityDate);
        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<CustomerBase, Guid>(x => x.EntityKey);
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);


        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The date the customer was last active on the site
        /// </summary>
        [DataMember]
        public DateTime LastActivityDate
        {
            get { return _lastActivityDate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _lastActivityDate = value;
                    return _lastActivityDate;
                }, _lastActivityDate, LastActivityDateSelector);
            }
        }

        /// <summary>
        /// Entity key
        /// </summary>
        [DataMember]
        public Guid EntityKey
        {
            get { return _entityKey; }
            set {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _entityKey = value;
                    return _entityKey;
                }, _entityKey, EntityKeySelector);
            }
        }

        /// <summary>
        /// True/False indicating whether or not this customer is an anonymous customer 
        /// </summary>
        [IgnoreDataMember]
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// A collection to store custom/extended data for the customer
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get { return _extendedData; }
            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            _lastActivityDate = DateTime.Now;
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            _lastActivityDate = DateTime.Now;
        }
    }
}