namespace Merchello.Web.Search
{
    /// <summary>
    /// Defines a filter group manager.
    /// </summary>
    public interface IFilterGroupManager
    {
        /// <summary>
        /// Gets the <see cref="IProductFilterGroupQuery"/>.
        /// </summary>
        IProductFilterGroupQuery Product { get; }
    }
}