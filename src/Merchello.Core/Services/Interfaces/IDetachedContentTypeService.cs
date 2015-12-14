namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.DetachedContent;

    using Umbraco.Core.Services;

    /// <summary>
    /// Re DetachedContentTypeService.
    /// </summary>
    public interface IDetachedContentTypeService : IService
    {
        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> without saving it to the database.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        IDetachedContentType CreateDetachedContentType(EntityType entityType, Guid contentTypeKey, string name, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> without saving it to the database.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        IDetachedContentType CreateDetachedContentType(Guid entityTfKey, Guid contentTypeKey, string name, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> and saves to the database.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        IDetachedContentType CreateDetachedContentTypeWithKey(EntityType entityType, Guid contentTypeKey, string name, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> and saves to the database.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        IDetachedContentType CreateDetachedContentTypeWithKey(Guid entityTfKey, Guid contentTypeKey, string name, bool raiseEvents = true);

        /// <summary>
        /// Saves a single instance of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Save(IDetachedContentType detachedContentType, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentTypes">
        /// The collection to be saved.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Save(IEnumerable<IDetachedContentType> detachedContentTypes, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single instance of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        void Delete(IDetachedContentType detachedContentType, bool raiseEvents = true);

        ///// <summary>
        ///// Deletes a collection of <see cref="IDetachedContentType"/>.
        ///// </summary>
        ///// <param name="detachedContentTypes">
        ///// The collection to be deleted.
        ///// </param>
        ///// <param name="raiseEvents">
        ///// Optional boolean indicating whether or not to raise events.
        ///// </param>
        //void Delete(IEnumerable<IDetachedContentType> detachedContentTypes, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IDetachedContentType"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        IDetachedContentType GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IDetachedContentType"/> by the entity type key.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        IEnumerable<IDetachedContentType> GetDetachedContentTypesByEntityTfKey(Guid entityTfKey);

        /// <summary>
        /// Gets a collection of <see cref="IDetachedContentType"/> by the Umbraco content type key.
        /// </summary>
        /// <param name="contentTypeKey">
        /// The Umbraco content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        IEnumerable<IDetachedContentType> GetDetachedContentTypesByContentTypeKey(Guid contentTypeKey);

        /// <summary>
        /// Gets all <see cref="IDetachedContentType"/>s.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        IEnumerable<IDetachedContentType> GetAll();
    }
}