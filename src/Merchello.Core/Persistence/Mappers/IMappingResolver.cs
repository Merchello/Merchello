namespace Merchello.Core.Persistence.Mappers
{
    using System;

    /// <summary>
    /// Represents a resolver for mapping properties between entities and DTO (POCO) classes.
    /// </summary>
    internal interface IMappingResolver
    {
        /// <summary>
        /// Return a mapper by type
        /// </summary>
        /// <param name="type">The entity type</param>
        /// <returns>The mapper responsible for mapping properties.</returns>
        BaseMapper ResolveMapperByType(Type type);
    }
}