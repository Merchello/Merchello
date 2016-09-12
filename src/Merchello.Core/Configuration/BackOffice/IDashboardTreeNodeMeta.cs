namespace Merchello.Core.Configuration.BackOffice
{

    /// <summary>
    /// Represents a back office tree.
    /// </summary>
    public interface IDashboardTreeNodeMeta : IDashboardTreeNodeLink
    {
        /// <summary>
        /// Gets the id for the parent route.
        /// </summary>
        string ParentRouteId { get; }

        /// <summary>
        /// Gets the id for the route to dashboard or section represented.
        /// </summary>
        string RouteId { get; }

        /// <summary>
        /// Gets the actual path for the route.
        /// </summary>
        string RoutePath { get; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        int SortOrder { get; }
    }
}