namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The queryable entity collection provider base.
    /// </summary>
    public abstract class QueryableEntityCollectionProviderBase : EntityCollectionProviderBase, IQueryableEntityCollectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryableEntityCollectionProviderBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        protected QueryableEntityCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }


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
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> GetEntities<T>(Dictionary<string, object> args) where T : class, IEntity
        {
            this.ValidateType(typeof(T));

            return this.GetEntities(args).Select(x => x as T);
        }

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public abstract IEnumerable<object> GetEntities(Dictionary<string, object> args);
    }
}