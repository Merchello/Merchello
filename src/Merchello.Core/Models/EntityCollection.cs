namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The entity collection.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class EntityCollection : Entity, IEntityCollection
    {
        /// <summary>
        /// The entity type field key selector.
        /// </summary>
        private static readonly PropertyInfo EntityTfKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid>(x => x.EntityTfKey);

        /// <summary>
        /// The name selector.
        /// </summary>
        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<EntityCollection, string>(x => x.Name);

        /// <summary>
        /// The provider key selector.
        /// </summary>
        private static readonly PropertyInfo ProviderKeySelector = ExpressionHelper.GetPropertyInfo<EntityCollection, Guid>(x => x.ProviderKey);

        /// <summary>
        /// The entity type field key.
        /// </summary>
        private Guid _entityTfKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The dynamic collection.
        /// </summary>
        private Guid _prodiverKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection"/> class.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="providerKey">
        /// The provider Key.
        /// </param>
        public EntityCollection(Guid entityTfKey, Guid providerKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _entityTfKey = entityTfKey;
            _prodiverKey = providerKey;
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
        /// Gets or sets the dynamic collection.
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get
            {
                return _prodiverKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                   o =>
                   {
                       _prodiverKey = value;
                       return _prodiverKey;
                   },
                   _prodiverKey,
                   ProviderKeySelector);
            }
        }
    }
}