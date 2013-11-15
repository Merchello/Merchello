using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a customer base class
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class CustomerBase : Entity, ICustomerBase
    {
        private DateTime _lastActivityDate;
        private Guid _entityKey;

        protected CustomerBase(bool isAnonymous)
        {
            IsAnonymous = isAnonymous;
        }

        private static readonly PropertyInfo LastActivityDateSelector = ExpressionHelper.GetPropertyInfo<CustomerBase, DateTime>(x => x.LastActivityDate);
        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<CustomerBase, Guid>(x => x.EntityKey);

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