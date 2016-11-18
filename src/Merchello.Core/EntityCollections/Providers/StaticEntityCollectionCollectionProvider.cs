namespace Merchello.Core.EntityCollections.Providers
{
    using System;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The static entity collection collection provider.
    /// </summary>
    /// TODO REMOVE THIS CLASS and fix tests
    [EntityCollectionProvider("A8120A01-E9BF-4204-ADDD-D9553F6F24FE", "1607D643-E5E8-4A93-9393-651F83B5F1A9", "Static Customer Collection", "A static customer collection that could be used for categorizing or grouping sales", false)]
    internal sealed class StaticEntityCollectionCollectionProvider : EntityCollectionProviderBase<IEntityCollection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticEntityCollectionCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public StaticEntityCollectionCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// The perform exists.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool PerformExists(IEntityCollection entity)
        {
            return MerchelloContext.Services.EntityCollectionService.ExistsInCollection(CollectionKey, entity.Key);
        }

        /// <summary>
        /// The perform get paged entities.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IEntityCollection}"/>.
        /// </returns>
        protected override Page<IEntityCollection> PerformGetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            return MerchelloContext.Services.EntityCollectionService.GetFromCollection(
                CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }
    }
}