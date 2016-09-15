namespace Merchello.Core.Persistence.Mappers
{
    using System;

    /// <summary>
    /// An attribute used to decorate mappers to be associated with entities
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class MapperForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperForAttribute"/> class.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        public MapperForAttribute(Type entityType)
        {
            this.EntityType = entityType;
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public Type EntityType { get; private set; }
    }
}