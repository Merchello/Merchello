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
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

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
                SetPropertyValueAndDetectChanges(value, ref _versionKey, _ps.Value.VersionKeySelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The version key selector.
            /// </summary>
            public readonly PropertyInfo VersionKeySelector = ExpressionHelper.GetPropertyInfo<VersionTaggedEntity, Guid>(x => x.VersionKey);
        }
    }
}