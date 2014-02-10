using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Merchello.Core.Models.EntityBase
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class VersionTaggedEntity : Entity, IVersionTaggedEntity
    {
        private Guid _versionKey;

        private static readonly PropertyInfo VersionKeySelector = ExpressionHelper.GetPropertyInfo<VersionTaggedEntity, Guid>(x => x.VersionKey);

        /// <summary>
        /// The version of the item cache
        /// </summary>
        /// <remarks>
        /// Used to track changes in some ItemCacheCollections
        /// </remarks>
        [DataMember]
        public Guid VersionKey
        {
            get { return _versionKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _versionKey = value;
                    return _versionKey;
                }, _versionKey, VersionKeySelector);
            }
        }

    }
}