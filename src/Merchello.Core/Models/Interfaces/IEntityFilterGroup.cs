namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines the entity filter grounp.  Used for filtering entities.
    /// </summary>
    public interface IEntityFilterGroup : IEntityCollection
    {
        /// <summary>
        /// Gets the attribute collections.
        /// </summary>
        EntityFilterCollection Filters { get; } 
    }
}