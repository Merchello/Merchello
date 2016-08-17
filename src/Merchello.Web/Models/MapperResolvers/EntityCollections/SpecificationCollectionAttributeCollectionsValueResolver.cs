namespace Merchello.Web.Models.MapperResolvers.EntityCollections
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.ContentEditing.Collections;

    /// <summary>
    /// A AutoMapper value resolver for mapping <see cref="EntitySpecificationAttributeCollection"/> to <see cref="IEnumerable{EntityCollectionDisplay}"/>.
    /// </summary>
    public class SpecificationCollectionAttributeCollectionsValueResolver : ValueResolver<IEntitySpecificationCollection, IEnumerable<EntityCollectionDisplay>>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionDisplay}"/>.
        /// </returns>
        protected override IEnumerable<EntityCollectionDisplay> ResolveCore(IEntitySpecificationCollection source)
        {
            return !source.AttributeCollections.Any() ? 
                Enumerable.Empty<EntityCollectionDisplay>() : 
                source.AttributeCollections.Select(x => x.ToEntityCollectionDisplay());
        }
    }
}