namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;
    using Core;
    using Core.Models.Interfaces;
    using Core.Models.TypeFields;

    /// <summary>
    /// The entity type resolver.
    /// </summary>
    public class EntityTypeResolver : ValueResolver<IAuditLog, EntityType>
    {
        /// <summary>
        /// Resolves the EntityType Enumeration
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="EntityType"/>.
        /// </returns>
        protected override EntityType ResolveCore(IAuditLog source)
        {
            return source.EntityTfKey != null ? EnumTypeFieldConverter.EntityType.GetTypeField(source.EntityTfKey.Value) : EntityType.Custom;
        }
    }
}