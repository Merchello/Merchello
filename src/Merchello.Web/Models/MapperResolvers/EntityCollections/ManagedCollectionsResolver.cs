namespace Merchello.Web.Models.MapperResolvers.EntityCollections
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing.Collections;

    /// <summary>
    /// The managed collections resolver.
    /// </summary>
    public class ManagedCollectionsResolver : ValueResolver<EntityCollectionProviderAttribute, IEnumerable<EntityCollectionDisplay>>
    {
        /// <summary>
        /// The entity collection service.
        /// </summary>
        private readonly IEntityCollectionService _entityCollectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedCollectionsResolver"/> class.
        /// </summary>
        public ManagedCollectionsResolver()
        {
            _entityCollectionService = MerchelloContext.Current.Services.EntityCollectionService;
        }

        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionDisplay}"/>.
        /// </returns>
        protected override IEnumerable<EntityCollectionDisplay> ResolveCore(EntityCollectionProviderAttribute source)
        {
            return _entityCollectionService.GetByProviderKey(source.Key).Select(x => x.ToEntityCollectionDisplay());
        }
    }
}