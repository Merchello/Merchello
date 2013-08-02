using System;
using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Core.Models.EntityBase;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Base entity class to provide a marker to either override internal "Umbraco" Entity.  
    /// </summary>
    public abstract class MerchelloEntity : Entity
    {
        
        internal virtual void ResetIdentity()
        {
            HasIdentity = false;
            Id = default(int);
        }

        internal virtual void MarkHasIdentity()
        {
            HasIdentity = true;
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
        /// Method to call on entity saved/updated
        /// </summary>
        internal virtual void UpdatingEntity()
        {
            UpdateDate = DateTime.Now;
        }

        

        /// <summary>
        /// Used by inheritors to set the value of properties, this will detect if the property value actually changed and if it did
        /// it will ensure that the property has a dirty flag set.
        ///</summary>  
        /// <remarks>
        /// This method is internal to <see cref="TracksChangesEntityBase"/>
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
