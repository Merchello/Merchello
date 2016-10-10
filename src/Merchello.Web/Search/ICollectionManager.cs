namespace Merchello.Web.Search
{
    /// <summary>
    /// Defines a collection manager.
    /// </summary>
    public interface ICollectionManager
    {
        /// <summary>
        /// Gets the <see cref="IProductFilterGroupQuery"/>.
        /// </summary>
        IProductCollectionQuery Product { get; }
    }
}