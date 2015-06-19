namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// The back office tree display.
    /// </summary>
    public class BackOfficeTreeDisplay
    {
        /// <summary>
        /// Gets or sets the route id.
        /// </summary>
        public string RouteId { get; set; }

        /// <summary>
        /// Gets or sets the parent route id.
        /// </summary>
        public string ParentRouteId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the route path.
        /// </summary>
        public string RoutePath { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }
    }
}