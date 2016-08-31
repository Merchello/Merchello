namespace Merchello.Core.EntityCollections.Providers
{
    using System;

    /// <summary>
    /// The static product collection provider.
    /// </summary>
    [EntityCollectionProvider("4700456D-A872-4721-8455-1DDAC19F8C16", "9F923716-A022-4089-A110-1E9B4E1F2AD1", "Static Product Collection", "A static product collection that could be used for product categories and product groupings", false)]
    internal sealed class StaticProductCollectionProvider : StaticProductCollectionProviderBase, IProductCollectionProvider
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
    }
}