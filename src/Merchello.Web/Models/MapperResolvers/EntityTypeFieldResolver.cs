namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The entity type field resolver.
    /// </summary>
    public class EntityTypeFieldResolver : ValueResolver<IHasEntityTypeField, TypeField>
    {
        /// <summary>
        /// Resolves the type field.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="TypeField"/>.
        /// </returns>
        protected override TypeField ResolveCore(IHasEntityTypeField source)
        {
            return (TypeField)source.GetTypeField();
        }
    }
}