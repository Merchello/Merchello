namespace Merchello.Core.EntityCollections
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence.Querying;

    using umbraco;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// The entity collection provider base.
    /// </summary>
    public abstract class EntityCollectionProviderBase : IEntityCollectionProvider
    {
        /// <summary>
        /// The entity collection.
        /// </summary>
        private Lazy<IEntityCollection> _entityCollection; 

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection Key.
        /// </param>
        protected EntityCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(collectionKey), "collectionKey");
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            MerchelloContext = merchelloContext;
            this.CollectionKey = collectionKey;
            this.Initialize();
        }


        /// <summary>
        /// Gets the entity collection.
        /// </summary>
        public IEntityCollection EntityCollection
        {
            get
            {
                return _entityCollection.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>.
        /// </summary>
        protected IMerchelloContext MerchelloContext { get; private set; }


        /// <summary>
        /// Gets the collection key.
        /// </summary>
        protected Guid CollectionKey { get; private set; }

        /// <summary>
        /// Ensures this is the provider by <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool EnsureType(Type type)
        {
            return EntityCollection.TypeOfEntities() == type;
        }

        /// <summary>
        /// Ensures this is the provider for the <see cref="EntityType"/>.
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws an exception if the EntityCollectionProviderAttribute was not found
        /// </exception>
        /// <remarks>
        /// Used in classes such as the MerchelloHelper
        /// </remarks>
        internal bool EnsureEntityType(EntityType entityType)
        {
            var att = this.ProviderAttribute();

            if (att == null)
            {
                var nullReference =
                    new NullReferenceException(
                        "EntityCollectionProvider was not decorated with an EntityCollectionProviderAttribute");
                LogHelper.Error<EntityCollectionProviderBase>("Provider must be decorated with an attribute", nullReference);
                throw nullReference;
            }

            return att.EntityTfKey.Equals(EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey);
        }        

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        private void Initialize()
        {
            _entityCollection = new Lazy<IEntityCollection>(() => MerchelloContext.Services.EntityCollectionService.GetByKey(CollectionKey));
        }
    }
}