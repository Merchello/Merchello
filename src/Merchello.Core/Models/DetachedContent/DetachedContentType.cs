namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;

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
        /// The description selector.
        /// </summary>
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, string>(x => x.Description);

        /// <summary>
        /// The content type id selector.
        /// </summary>
        private static readonly PropertyInfo ContentTypeKeySelector = ExpressionHelper.GetPropertyInfo<DetachedContentType, Guid?>(x => x.ContentTypeKey);

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
            Mandate.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityTfKey");
            this._contentTypeKey = contentTypeKey;
            this._entityTfKey = entityTfKey;
        }

        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [DataMember]
        public Guid EntityTfKey
        {
            get
            {
                return this._entityTfKey;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        this._entityTfKey = value;
                        return this._entityTfKey;
                    },
                    this._entityTfKey,
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
                return this._name;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        this._name = value;
                        return this._name;
                    },
                    this._name,
                    NameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [IgnoreDataMember]
        public string Description
        {
            get
            {
                return this._description;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        this._description = value;
                        return this._description;
                    },
                    this._description,
                    DescriptionSelector);
            }
        }

        /// <summary>
        /// Gets or sets the content type key.
        /// </summary>
        [DataMember]
        public Guid? ContentTypeKey
        {
            get
            {
                return this._contentTypeKey;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        this._contentTypeKey = value;
                        return this._contentTypeKey;
                    },
                    this._contentTypeKey,
                    ContentTypeKeySelector);
            }
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        [IgnoreDataMember]
        public EntityType EntityType
        {
            get
            {
                return EnumTypeFieldConverter.EntityType.GetTypeField(EntityTfKey);
            }
        }
    }
}