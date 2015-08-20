namespace Merchello.Core.EntityCollections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// The entity collection base extensions.
    /// </summary>
    public static class EntityCollectionBaseExtensions
    {
        /// <summary>
        /// Gets the <see cref="EntityCollectionProviderAttribute"/>.
        /// </summary>
        /// <param name="provider">
        /// The entity collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderAttribute"/>.
        /// </returns>
        public static EntityCollectionProviderAttribute ProviderAttribute(this EntityCollectionProviderBase provider)
        {
            return provider.GetType().GetCustomAttribute<EntityCollectionProviderAttribute>(false);
        }

        /// <summary>
        /// The get managed collections.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IEntityCollection> GetManagedCollections(this EntityCollectionProviderBase provider)
        {
            var att = provider.ProviderAttribute();

            if (!MerchelloContext.HasCurrent || att == null) return Enumerable.Empty<IEntityCollection>();

            return MerchelloContext.Current.Services.EntityCollectionService.GetByProviderKey(att.Key);
        }
    }
}