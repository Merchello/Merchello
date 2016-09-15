namespace Merchello.Core.Persistence.Mappers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using LightInject;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.ObjectResolution;
    using Merchello.Core.Logging;

    /// <summary>
    /// Represents a resolver for mapping entity to DTO (POCO) mappers.
    /// </summary>
    internal class MappingResolver : ContainerLazyManyObjectsResolver<MappingResolver, BaseMapper>, IMappingResolver
    {
        /// <summary>
        /// Caches the type -> mapper so that we don't have to type check each time we want one or lookup the attribute
        /// </summary>
        private readonly ConcurrentDictionary<Type, BaseMapper> _mapperCache = new ConcurrentDictionary<Type, BaseMapper>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingResolver"/> class. 
        /// Constructor accepting a list of BaseMapper types that are attributed with the MapperFor attribute
        /// </summary>
        /// <param name="container">
        /// The <see cref="IServiceContainer"/>
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/>
        /// </param>
        /// <param name="assignedMapperTypes">
        /// The mapper types that can be assigned
        /// </param>
        public MappingResolver(IServiceContainer container, ILogger logger, Func<IEnumerable<Type>> assignedMapperTypes)
            : base(container, logger, assignedMapperTypes)
        {
        }

        /// <summary>
        /// Returns a mapper for the entity type
        /// </summary>
        /// <param name="type">The type of the entity</param>
        /// <returns>
        /// The mapper assigned to the entity
        /// </returns>
        public BaseMapper ResolveMapperByType(Type type)
        {
            return this._mapperCache.GetOrAdd(
                type, 
                type1 =>
                {
                    // first check if we can resolve it by attribute
                    var byAttribute = this.TryGetMapperByAttribute(type);
                    if (byAttribute.Success)
                    {
                        return byAttribute.Result;
                    }

                    throw new Exception("Invalid Type: A Mapper could not be resolved based on the passed in Type");
                });
        }

        /// <summary>
        /// Check the entity type to see if it has a mapper attribute assigned and try to instantiate it
        /// </summary>
        /// <param name="entityType">The type of the entity</param>
        /// <returns>An attempt representing the resolution of the mapper</returns>
        private Attempt<BaseMapper> TryGetMapperByAttribute(Type entityType)
        {
            // check if any of the mappers are assigned to this type
            var mapper = this.Values.FirstOrDefault(
                x => x.GetType().GetCustomAttributes<MapperForAttribute>(false)
                      .Any(m => m.EntityType == entityType));

            if (mapper == null)
            {
                return Attempt<BaseMapper>.Fail();
            }

            return Attempt<BaseMapper>.Succeed(mapper);
        }
    }
}