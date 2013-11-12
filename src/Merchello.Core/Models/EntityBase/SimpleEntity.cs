using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    [DataContract(IsReference = true)]
    public class SimpleEntity : ISimpleEntity
    {
        private int _id;
        private bool _hasIdentity;

        [DataMember]
        public int Id {
            get { return _id; }
            set
            {
                _id = value;
                _hasIdentity = true;
            }

        }

        /// <summary>
        /// Gets or sets the Created Date
        /// </summary>
        [DataMember]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the Update Date
        /// </summary>
        [DataMember]
        public DateTime UpdateDate { get; set; }


        /// <summary>
        /// Indicates whether the current entity has an identity, eg. Id.
        /// </summary>
        [IgnoreDataMember]
        public virtual bool HasIdentity
        {
            get { return _hasIdentity; }
            protected set
            {
                _hasIdentity = value;
            } 
        }

        internal virtual void ResetIdentity()
        {
            _hasIdentity = false;
        }

        /// <summary>
        /// Method to call on entity saved when first added
        /// </summary>
        internal virtual void AddingEntity()
        {
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Method to call on entity saved
        /// </summary>
        internal virtual void UpdatingEntity()
        {
            UpdateDate = DateTime.Now;
        }

    }
}