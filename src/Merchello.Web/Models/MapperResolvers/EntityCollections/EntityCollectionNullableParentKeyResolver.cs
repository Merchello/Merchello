namespace Merchello.Web.Models.MapperResolvers.EntityCollections
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using AutoMapper;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The entity collection nullable parent key resolver.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class EntityCollectionNullableParentKeyResolver : ValueResolver<IEntityCollection, Guid?>
    {
        /// <summary>
        /// Asserts the ParentKey returns null when an empty GUID or null.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The nullable Guid.
        /// </returns>
        protected override Guid? ResolveCore(IEntityCollection source)
        {
            return source.ParentKey == null || source.ParentKey == Guid.Empty ? null : source.ParentKey;
        }
    }
}