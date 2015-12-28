namespace Merchello.Core.Models.EntityBase
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an entity that is marked with a version GUID.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class VersionTaggedEntity : Entity, IVersionTaggedEntity
    {
        /// <summary>
        /// The version key selector.
        /// </summary>
        private static readonly PropertyInfo VersionKeySelector = ExpressionHelper.GetPropertyInfo<VersionTaggedEntity, Guid>(x => x.VersionKey);
        
        /// <summary>
        /// The version key.
        /// </summary>
        private Guid _versionKey;

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        [DataMember]
        public Guid VersionKey
        {
            get
            {
                return _versionKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _versionKey = value;
                    return _versionKey;
                }, 
                _versionKey, 
                VersionKeySelector);
            }
        }
    }
}