using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AnonymousCustomer :  KeyEntity, IAnonymousCustomer
    {

        private DateTime _lastActivityDate;

        public AnonymousCustomer(DateTime lastActivityDate)
        {
            _lastActivityDate = lastActivityDate;
        }

        /// <summary>
        /// Sets the last activity date for the anonymous customer
        /// </summary>
        /// <remarks>
        /// This is used in maintenance routines to clear orphaned anonymous customer records
        /// </remarks>
        [DataMember]
        public DateTime LastActivityDate
        {
            get
            {
                return _lastActivityDate;
            }          
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void AddingEntity()
        {
            base.AddingEntity();
            Key = Guid.NewGuid();
        }

        /// <summary>
        /// Asserts that the last activity date is set to the current date time
        /// </summary>
        internal override void UpdatingEntity()
        {
            base.UpdatingEntity();
            _lastActivityDate = DateTime.Now;
        }

        /// <summary>
        /// Indicates that this is an anonymous consumer
        /// </summary>
        [IgnoreDataMember]
        public bool IsAnonymous
        {
            get { return true; }
        }
        
    }
}
