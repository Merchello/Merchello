namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using umbraco.cms.presentation;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The static product collection provider.
    /// </summary>
    [EntityCollectionProvider("Static Product Collection", "A static product collection")]
    public class StaticProductCollectionProvider : EntityCollectionProviderBase<IProduct>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticProductCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public StaticProductCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// Gets the provider key.
        /// </summary>
        public override Guid ProviderKey
        {
            get
            {
                return Constants.ProviderKeys.EntityCollection.StaticProductCollectionProviderKey;
            }
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public override ITypeField EntityType
        {
            get
            {
                return EnumTypeFieldConverter.EntityType.Product;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this provider manages a single (unique) <see cref="IEntityCollection"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="StaticProductCollectionProvider"/> manages multiple product collections
        /// </remarks>
        public override bool ManagesUniqueCollection
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
        public override IEnumerable<IProduct> GetEntities()
        {
            return this.GetPagedEntities(1, long.MaxValue).Items;
        }

        /// <summary>
        /// The get entities.
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
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        public override Page<IProduct> GetPagedEntities(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending)
        {
            return MerchelloContext.Services.ProductService.GetProductsFromStaticCollection(
                CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// The get paged entity keys.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> GetPagedEntityKeys(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending)
        {
            return ((ProductService)MerchelloContext.Services.ProductService).GetProductKeysFromStaticCollection(
                CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }
    }
}