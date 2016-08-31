namespace Merchello.Web.Search
{
    using Merchello.Web.Services;

    /// <summary>
    /// Defines a collection manager.
    /// </summary>
    public interface ICollectionManager
    {
        /// <summary>
        /// Gets the <see cref="IProductFilterGroupService"/>.
        /// </summary>
        IProductCollectionService Product { get; }
    }
}