using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class Entity : IEntity
    {
        private Guid _key;
        private bool _hasIdentity;
        private bool _wasCancelled;
        private DateTime _createDate;
        private DateTime _updateDate;

        private static readonly PropertyInfo KeySelector = ExpressionHelper.GetPropertyInfo<Entity, Guid>(x => x.Key);
        private static readonly PropertyInfo WasCancelledSelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.WasCancelled);
        private static readonly PropertyInfo CreateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.CreateDate);
        private static readonly PropertyInfo UpdateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.UpdateDate);
        private static readonly PropertyInfo HasIdentitySelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.HasIdentity);
        
      
        #region IEntity Members        

        /// <summary>
        /// Guid based Id
        /// </summary>
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

#endregion


        #region IEntityBase Memebers


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
        [IgnoreDataMember]
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
        /// Method to call on entity saved when first added
        /// </summary>
        internal virtual void AddingEntity()
        {
            if (Key == Guid.Empty)
                _key = Guid.NewGuid(); // set the _key so that the HasIdentity flag is not set

            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Method to call on entity saved/updated
        /// </summary>
        internal virtual void UpdatingEntity()
        {
            UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Tracks the properties that have changed
        /// </summary>        
        private readonly IDictionary<string, bool> _propertyChangedInfo = new Dictionary<string, bool>();


        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method to call on a property setter.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        protected virtual void OnPropertyChanged(PropertyInfo propertyInfo)
        {
            _propertyChangedInfo[propertyInfo.Name] = true;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyInfo.Name));
            }
        }

        #region ITracksDirty Members

        /// <summary>
        /// Indicates whether the current entity is dirty.
        /// </summary>
        /// <returns>True if entity is dirty, otherwise False</returns>
        public bool IsDirty()
        {
            return _propertyChangedInfo.Any();
        }

        /// <summary>
        /// Indicates whether a specific property on the current entity is dirty.
        /// </summary>
        /// <param name="propertyName">Name of the property to check</param>
        /// <returns>True if Property is dirty, otherwise False</returns>
        public bool IsPropertyDirty(string propertyName)
        {
            return _propertyChangedInfo.Any(x => x.Key == propertyName);
        }

        /// <summary>
        /// Resets dirty properties by clearing the dictionary used to track changes.
        /// </summary>
        public virtual void ResetDirtyProperties()
        {
            _propertyChangedInfo.Clear();
        }

        #endregion

        /// <summary>
        /// Used by inheritors to set the value of properties, this will detect if the property value actually changed and if it did
        /// it will ensure that the property has a dirty flag set.
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="value"></param>
        /// <param name="propertySelector"></param>
        /// <returns>returns true if the value changed</returns>
        /// <remarks>
        /// This is required because we don't want a property to show up as "dirty" if the value is the same. For example, when we 
        /// save a document type, nearly all properties are flagged as dirty just because we've 'reset' them, but they are all set 
        /// to the same value, so it's really not dirty.
        /// </remarks>
        internal bool SetPropertyValueAndDetectChanges<T>(Func<T, T> setValue, T value, PropertyInfo propertySelector)
        {
            var initVal = value;
            var newVal = setValue(value);
            if (!Equals(initVal, newVal))
            {
                OnPropertyChanged(propertySelector);
                return true;
            }
            return false;
        }


        public virtual bool SameIdentityAs(IEntity other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return SameIdentityAs(other as Entity);
        }

        public virtual bool SameIdentityAs(Entity other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() == other.GetRealType() && HasIdentity && other.HasIdentity)
                return other.Key.Equals(Key);

            return false;
        }

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
