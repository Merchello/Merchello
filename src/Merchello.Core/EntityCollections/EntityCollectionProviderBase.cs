namespace Merchello.Core.EntityCollections
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The entity collection provider base.
    /// </summary>
    public abstract class EntityCollectionProviderBase
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
        /// Initializes the provider.
        /// </summary>
        private void Initialize()
        {
            _entityCollection = new Lazy<IEntityCollection>(() => MerchelloContext.Services.EntityCollectionService.GetByKey(CollectionKey));
        }
    }
}