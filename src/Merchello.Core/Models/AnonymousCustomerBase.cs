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
    public abstract class AnonymousCustomerBase : Entity, ICustomerBase
    {
        private DateTime _lastActivityDate;

        protected AnonymousCustomerBase(bool isAnonymous)
        {
            IsAnonymous = isAnonymous;
        }

        private static readonly PropertyInfo LastActivityDateSelector = ExpressionHelper.GetPropertyInfo<AnonymousCustomerBase, DateTime>(x => x.LastActivityDate);


        public Guid EntityKey { get { return Key; } }

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