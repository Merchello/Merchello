namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Marker interface for an EntityCollectionProvider.
    /// </summary>
    public interface IEntityCollectionProvider
    {
        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        IEnumerable<object> GetEntities();

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<object> GetEntities(Dictionary<string, object> args);
            
            /// <summary>
        /// The get entities.
        /// </summary>
        /// <typeparam name="T">
        /// The type of IEntity
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<T> GetEntities<T>() where T : class, IEntity;

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<T> GetEntities<T>(Dictionary<string, object> args) where T : class, IEntity;
    }
}