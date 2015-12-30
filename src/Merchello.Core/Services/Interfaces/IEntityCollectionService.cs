namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Services;

    /// <summary>
    /// The EntityCollectionService interface.
    /// </summary>
    public interface IEntityCollectionService : IUaaSItemProviderService
    {
        /// <summary>
        /// The create entity collection.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        IEntityCollection CreateEntityCollection(
            EntityType entityType,
            Guid providerKey,
            string name,
            bool raiseEvents = true);

        /// <summary>
        /// The create entity collection with key.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        IEntityCollection CreateEntityCollectionWithKey(
            EntityType entityType,
            Guid providerKey,
            string name,
            bool raiseEvents = true);


        ///// <summary>
        ///// The create entity collection.
        ///// </summary>
        ///// <param name="entityTfKey">
        ///// The entity type field key.
        ///// </param>
        ///// <param name="providerKey">
        ///// The provider key.
        ///// </param>
        ///// <param name="name">
        ///// The name.
        ///// </param>
        ///// <param name="raiseEvents">
        ///// Optional boolean indicating whether or not to raise events
        ///// </param>
        ///// <returns>
        ///// The <see cref="IEntityCollection"/>.
        ///// </returns>
        //IEntityCollection CreateEntityCollection(Guid entityTfKey, Guid providerKey, string name, bool raiseEvents = true);

        ///// <summary>
        ///// The create entity collection with key.
        ///// </summary>
        ///// <param name="entityTfKey">
        ///// The entity type field key.
        ///// </param>
        ///// <param name="providerKey">
        ///// The provider key.
        ///// </param>
        ///// <param name="name">
        ///// The name.
        ///// </param>
        ///// <param name="raiseEvents">
        ///// Optional boolean indicating whether or not to raise events
        ///// </param>
        ///// <returns>
        ///// The <see cref="IEntityCollection"/>.
        ///// </returns>
        //IEntityCollection CreateEntityCollectionWithKey(Guid entityTfKey, Guid providerKey, string name, bool raiseEvents = true);

        /// <summary>
        /// Saves a single entity collection.
        /// </summary>
        /// <param name="entityCollection">
        /// The entity collection.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IEntityCollection entityCollection, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of entity collections.
        /// </summary>
        /// <param name="entityCollections">
        /// The entity collections.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void Save(IEnumerable<IEntityCollection> entityCollections, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single entity collection.
        /// </summary>
        /// <param name="entityCollection">
        /// The entity collection.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Delete(IEntityCollection entityCollection, bool raiseEvents = true);

        ///// <summary>
        ///// Deletes a collection of entity collections.
        ///// </summary>
        ///// <param name="entityCollections">
        ///// The entity collections.
        ///// </param>
        ///// <param name="raiseEvents">
        ///// Optional boolean indicating whether or not to raise events.
        ///// </param>
        ///// <remarks>
        ///// This has to be internal due to resetting of sort orders
        ///// </remarks>
        //void Delete(IEnumerable<IEntityCollection> entityCollections, bool raiseEvents = true);

        /// <summary>
        /// The get by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        IEntityCollection GetByKey(Guid key);

        /// <summary>
        /// The get by entity type field key.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetByEntityTfKey(Guid entityTfKey);

        /// <summary>
        /// The get by provider key.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetByProviderKey(Guid providerKey);

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetAll();

        /// <summary>
        /// The get children.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetChildren(Guid collectionKey);

        /// <summary>
        /// The exists in collection.
        /// </summary>
        /// <param name="parentKey">
        /// The parent key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ExistsInCollection(Guid? parentKey, Guid collectionKey);

        /// <summary>
        /// The get root level entity collections.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetRootLevelEntityCollections();

        /// <summary>
        /// The get root level entity collections.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetRootLevelEntityCollections(EntityType entityType);

        /// <summary>
        /// The get root level entity collections.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity tf key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        IEnumerable<IEntityCollection> GetRootLevelEntityCollections(Guid entityTfKey);


        /// <summary>
        /// Gets a Page of collections from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<IEntityCollection> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The child entity collection count.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection Key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int ChildEntityCollectionCount(Guid collectionKey);

        /// <summary>
        /// The has child entity collections.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool HasChildEntityCollections(Guid collectionKey);
    }
}