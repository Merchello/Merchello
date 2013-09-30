using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models.EntityBase
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class Entity : MerchelloEntityBase, ISingularRoot
    {
        private int _id;
        private Guid _key;
        private bool _hasIdentity;
        private bool _wasCancelled;
        private DateTime _createDate;
        private DateTime _updateDate;

        private static readonly PropertyInfo IdSelector = ExpressionHelper.GetPropertyInfo<Entity, int>(x => x.Id);
        private static readonly PropertyInfo KeySelector = ExpressionHelper.GetPropertyInfo<Entity, Guid>(x => x.Key);
        private static readonly PropertyInfo WasCancelledSelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.WasCancelled);
        private static readonly PropertyInfo CreateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.CreateDate);
        private static readonly PropertyInfo UpdateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.UpdateDate);
        private static readonly PropertyInfo HasIdentitySelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.HasIdentity);

        #region IEntity Members        

        /// <summary>
        /// Integer Id
        /// </summary>
        [DataMember]
        public virtual int Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _id = value;
                    HasIdentity = true; //set the has Identity
                    return _id;
                }, _id, IdSelector);
            }
        }

        /// <summary>
        /// Guid based Id
        /// </summary>
        /// <remarks>This is different than the Umbraco.Core EntityEntity in that this key is used by several tables as the primary
        /// key in lieu of the Id
        /// </remarks>
        [DataMember]
        public virtual Guid Key
        {
            get
            {
                return _key;
            }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _key = value;
                    HasIdentity = true;
                    return _key;
                }, _key, KeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the Created Date
        /// </summary>
        [DataMember]
        public DateTime CreateDate
        {
            get { return _createDate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _createDate = value;
                    return _createDate;
                }, _createDate, CreateDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets the Modified Date
        /// </summary>
        [DataMember]
        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _updateDate = value;
                    return _updateDate;
                }, _updateDate, UpdateDateSelector);
            }
        }

        /// <summary>
        /// Indicates whether the current entity has an identity, eg. Id.
        /// </summary>
        public virtual bool HasIdentity
        {
            get
            {
                return _hasIdentity;
            }
            protected set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _hasIdentity = value;
                    return _hasIdentity;
                }, _hasIdentity, HasIdentitySelector);
            }
        }

        #endregion

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
        /// Gets or sets the WasCancelled flag, which is used to track
        /// whether some action against an entity was cancelled through some event.
        /// </summary>
        [IgnoreDataMember]
        internal bool WasCancelled
        {
            get { return _wasCancelled; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _wasCancelled = value;
                    return _wasCancelled;
                }, _wasCancelled, WasCancelledSelector);
            }
        }

        /// <summary>
        /// Method to call on entity saved/updated
        /// </summary>
        internal virtual void UpdatingEntity()
        {
            UpdateDate = DateTime.Now;
        }

        public abstract bool SameIdentityAs(IEntity other);

        public virtual bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return SameIdentityAs(other);
        }

        public virtual Type GetRealType()
        {
            return GetType();
        }
    }
}
