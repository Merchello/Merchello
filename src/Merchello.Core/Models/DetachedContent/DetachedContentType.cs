namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The detached content type.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class DetachedContentType : Entity, IDetachedContentType
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The entity type field key.
        /// </summary>
        private Guid _entityTfKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The content type id.
        /// </summary>
        private Guid? _contentTypeKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentType"/> class.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content Type Key.
        /// </param>
        public DetachedContentType(Guid entityTfKey, Guid? contentTypeKey)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityTfKey");
            this._contentTypeKey = contentTypeKey;
            this._entityTfKey = entityTfKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid EntityTfKey
        {
            get
            {
                return this._entityTfKey;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _entityTfKey, _ps.Value.EntityTfKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _description, _ps.Value.DescriptionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? ContentTypeKey
        {
            get
            {
                return this._contentTypeKey;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _contentTypeKey, _ps.Value.ContentTypeKeySelector);
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        public EntityType EntityType
        {
            get
            {
                return EnumTypeFieldConverter.EntityType.GetTypeField(EntityTfKey);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The entity type field key selector.
            /// </summary>
            public readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, Guid>(x => x.EntityTfKey);

            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, string>(x => x.Name);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, string>(x => x.Description);

            /// <summary>
            /// The content type id selector.
            /// </summary>
            public readonly PropertyInfo ContentTypeKeySelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, Guid?>(x => x.ContentTypeKey);
        }
    }
}