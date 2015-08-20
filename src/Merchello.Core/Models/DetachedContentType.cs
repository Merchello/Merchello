namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The detached content type.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class DetachedContentType : Entity, IDetachedContentType
    {
        /// <summary>
        /// The entity type field key selector.
        /// </summary>
        private static readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, Guid>(x => x.EntityTfKey);

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, string>(x => x.Name);

        /// <summary>
        /// The content type id selector.
        /// </summary>
        private static readonly PropertyInfo ContentTypeIdSelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, int?>(x => x.ContentTypeId);

        /// <summary>
        /// The entity type field key.
        /// </summary>
        private Guid _entityTfKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The content type id.
        /// </summary>
        private int? _contentTypeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentType"/> class.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="contentTypeId">
        /// The content Type Id.
        /// </param>
        public DetachedContentType(Guid entityTfKey, int? contentTypeId)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityTfKey");
            _contentTypeId = contentTypeId;
            _entityTfKey = entityTfKey;
        }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        public Guid EntityTfKey
        {
            get
            {
                return _entityTfKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _entityTfKey = value;
                        return _entityTfKey;
                    },
                    _entityTfKey,
                    EntityTfKeySelector);
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _name = value;
                        return _name;
                    },
                    _name,
                    NameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the content type id.
        /// </summary>
        [DataMember]
        public int? ContentTypeId
        {
            get
            {
                return _contentTypeId;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _contentTypeId = value;
                        return _contentTypeId;
                    },
                    _contentTypeId,
                    ContentTypeIdSelector);
            }
        }
    }
}