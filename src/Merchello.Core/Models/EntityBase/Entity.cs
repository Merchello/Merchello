namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Plugins;

    /// <summary>
    /// Defines a Merchello entity.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class Entity : IEntity
    {
        #region Fields

        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

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
                SetPropertyValueAndDetectChanges(value, ref _createDate, _ps.Value.CreateDateSelector);
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
                SetPropertyValueAndDetectChanges(value, ref _updateDate, _ps.Value.UpdateDateSelector);
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
                SetPropertyValueAndDetectChanges(value, ref _hasIdentity, _ps.Value.HasIdentitySelector);
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
                SetPropertyValueAndDetectChanges(value, ref _key, _ps.Value.KeySelector);
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
                SetPropertyValueAndDetectChanges(value, ref _wasCancelled, _ps.Value.WasCancelledSelector);
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
        /// <typeparam name="T">
        /// The type of the property
        /// </typeparam>
        /// <param name="newVal">
        /// The new value
        /// </param>
        /// <param name="origVal">
        /// The original value
        /// </param>
        /// <param name="propertySelector">
        /// The property selector
        /// </param>
        /// <remarks>
        /// This is required because we don't want a property to show up as "dirty" if the value is the same. For example, when we
        /// save a document type, nearly all properties are flagged as dirty just because we've 'reset' them, but they are all set
        /// to the same value, so it's really not dirty.
        /// </remarks>
        internal void SetPropertyValueAndDetectChanges<T>(T newVal, ref T origVal, PropertyInfo propertySelector)
        {
            if ((typeof(T) == typeof(string) == false) && TypeHelper.IsTypeAssignableFrom<IEnumerable>(typeof(T)))
            {
                throw new InvalidOperationException("This method does not support IEnumerable instances. For IEnumerable instances a manual custom equality check will be required");
            }

            SetPropertyValueAndDetectChanges(newVal, ref origVal, propertySelector, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// The set property value and detect changes.
        /// </summary>
        /// <param name="newVal">
        /// The new value.
        /// </param>
        /// <param name="origVal">
        /// The original value.
        /// </param>
        /// <param name="propertySelector">
        /// The property selector.
        /// </param>
        /// <param name="comparer">
        /// The equality comparer.
        /// </param>
        /// <typeparam name="T">
        /// The type of the property
        /// </typeparam>
        internal void SetPropertyValueAndDetectChanges<T>(T newVal, ref T origVal, PropertyInfo propertySelector, IEqualityComparer<T> comparer)
        {
            // check changed
            var changed = comparer.Equals(origVal, newVal) == false;

            // set the original value
            origVal = newVal;

            // raise the event if it was changed
            if (changed)
            {
                OnPropertyChanged(propertySelector);
            }
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

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The key selector.
            /// </summary>
            public readonly PropertyInfo KeySelector = ExpressionHelper.GetPropertyInfo<Entity, Guid>(x => x.Key);

            /// <summary>
            /// The was cancelled selector.
            /// </summary>
            public readonly PropertyInfo WasCancelledSelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.WasCancelled);

            /// <summary>
            /// The create date selector.
            /// </summary>
            public readonly PropertyInfo CreateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.CreateDate);

            /// <summary>
            /// The update date selector.
            /// </summary>
            public readonly PropertyInfo UpdateDateSelector = ExpressionHelper.GetPropertyInfo<Entity, DateTime>(x => x.UpdateDate);

            /// <summary>
            /// The has identity selector.
            /// </summary>
            public readonly PropertyInfo HasIdentitySelector = ExpressionHelper.GetPropertyInfo<Entity, bool>(x => x.HasIdentity);
        }
    }
}
