using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Merchello.Core.Models.EntityBase
{
    [Serializable]
    public abstract class MerchelloEntityBase : ITracksDirty
    {

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
        public void ResetDirtyProperties()
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
    }
}
