namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a Merchello entity.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class Entity : IEntity
    {
        #region Fields

        /// <summary>
        /// The key selector.
        /// </summary>
        private static readonly PropertyInfo KeySelector = ExpressionHelper.GetPropertyInfo<Entity, Guid>(x => x.Key);

        /// <summary>
        /// The was cancelled selector.
        /// </summary>
        private static readonly PropertyInfo WasCancelledSelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.WasCancelled);

        /// <summary>
        /// The create date selector.
        /// </summary>
        private static readonly PropertyInfo CreateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.CreateDate);

        /// <summary>
        /// The update date selector.
        /// </summary>
        private static readonly PropertyInfo UpdateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.UpdateDate);

        /// <summary>
        /// The has identity selector.
        /// </summary>
        private static readonly PropertyInfo HasIdentitySelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.HasIdentity);

        /// <summary>
        /// Tracks the properties that have changed
        /// </summary>        
        private readonly IDictionary<string, bool> _propertyChangedInfo = new Dictionary<string, bool>();

        /// <summary>
        /// The entity key.
        /// </summary>
        private Guid _key;

        /// <summary>
        /// A value that indicates whether or not this entity has an identity.
        /// </summary>
        private bool _hasIdentity;

        /// <summary>
        /// A value that indicates a CRUD operation was cancelled.
        /// </summary>
        private bool _wasCancelled;

        /// <summary>
        /// The create date.
        /// </summary>
        private DateTime _createDate;

        /// <summary>
        /// The update date.
        /// </summary>
        private DateTime _updateDate;

        #endregion

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the Created Date
        /// </summary>
        [DataMember]
        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                o =>
                {
                    _createDate = value;
                    return _createDate;
                }, 
                _createDate, 
                CreateDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets the Modified Date
        /// </summary>
        [DataMember]
        public DateTime UpdateDate
        {
            get
            {
                return _updateDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _updateDate = value;
                    return _updateDate;
                }, 
                _updateDate, 
                UpdateDateSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current entity has an identity
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
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _hasIdentity = value;
                        return _hasIdentity;
                    },
                _hasIdentity,
                HasIdentitySelector);
            }
        }

        /// <summary>
        /// Gets or sets the GUID based Id
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
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _key = value;
                        HasIdentity = true;
                        return _key;
                    },
                _key,
                KeySelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WasCancelled flag has been set, which is used to track
        /// whether some action against an entity was cancelled through some event.
        /// </summary>
        [IgnoreDataMember]
        internal bool WasCancelled
        {
            get
            {
                return _wasCancelled;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _wasCancelled = value;
                    return _wasCancelled;
                }, 
                _wasCancelled, 
                WasCancelledSelector);
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
        /// Utility "equals" method.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool SameIdentityAs(IEntity other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return SameIdentityAs(other as Entity);
        }

        /// <summary>
        /// Utility "equals" method.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the identity matches.
        /// </returns>
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

        /// <summary>
        /// Represents an equal comparison between to objects that sub class <see cref="Entity"/>.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Equals(Entity other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return SameIdentityAs(other);
        }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        public virtual Type GetRealType()
        {
            return GetType();
        }


        /// <summary>
        /// Special case override of default HasIdentity behavior when we want developers to be able to define their own "Keys" rather than
        /// allow the system to generate them.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <remarks>
        /// 
        /// GatewayProvider users this
        /// 
        /// </remarks>
        internal void ResetHasIdentity(bool value = false)
        {
            HasIdentity = value;
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
        /// Used by inheritors to set the value of properties, this will detect if the property value actually changed and if it did
        /// it will ensure that the property has a dirty flag set.
        /// </summary>
        /// <returns>
        /// returns true if the value changed
        /// </returns>
        /// <remarks>
        /// This is required because we don't want a property to show up as "dirty" if the value is the same. For example, when we 
        /// save a document type, nearly all properties are flagged as dirty just because we've 'reset' them, but they are all set 
        /// to the same value, so it's really not dirty.
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented", Justification = "Reviewed. Suppression is OK here."),
        SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1618:GenericTypeParametersMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        internal bool SetPropertyValueAndDetectChanges<T>(Func<T, T> setValue, T value, PropertyInfo propertySelector)
        {
            var initVal = value;
            var newVal = setValue(value);
            if (Equals(initVal, newVal))
            {
                return false;
            }

            this.OnPropertyChanged(propertySelector);
            return true;
        }  

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
    }
}
