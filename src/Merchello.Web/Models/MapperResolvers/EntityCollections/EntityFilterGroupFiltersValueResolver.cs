namespace Merchello.Web.Models.MapperResolvers.EntityCollections
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.ContentEditing.Collections;

    /// <summary>
    /// A AutoMapper value resolver for mapping <see cref="EntityFilterCollection"/> to <see cref="IEnumerable{EntityCollectionDisplay}"/>.
    /// </summary>
    public class EntityFilterGroupFiltersValueResolver : ValueResolver<IEntityFilterGroup, IEnumerable<EntityCollectionDisplay>>
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
        protected override IEnumerable<EntityCollectionDisplay> ResolveCore(IEntityFilterGroup source)
        {
            return !source.Filters.Any() ? 
                Enumerable.Empty<EntityCollectionDisplay>() : 
                source.Filters.Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.SortOrder);
        }
    }
}