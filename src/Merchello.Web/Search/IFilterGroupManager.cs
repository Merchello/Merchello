namespace Merchello.Web.Search
{
    using Merchello.Web.Services;

    /// <summary>
    /// Defines a filter group manager.
    /// </summary>
    public interface IFilterGroupManager
    {
        /// <summary>
        /// Gets the <see cref="IProductFilterGroupService"/>.
        /// </summary>
        IProductFilterGroupService Product { get; }
    }
}