namespace Merchello.Web.Trees
{
    using System;
    using System.Security.Permissions;

    using Merchello.Core;

    /// <summary>
    /// The report view attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BackOfficeTreeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficeTreeAttribute"/> class.
        /// </summary>
        /// <param name="routeId">
        /// The route Id.
        /// </param>
        /// <param name="parentRouteId">
        /// The parent Route Id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        /// <param name="routePath">
        /// The report back office route path.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        public BackOfficeTreeAttribute(string routeId, string parentRouteId, string title, string icon, string routePath, int sortOrder)
        {            
            Mandate.ParameterNotNullOrEmpty(routeId, "routeId");
            Mandate.ParameterNotNullOrEmpty(title, "title");
            Mandate.ParameterNotNullOrEmpty(icon, "icon");
            Mandate.ParameterNotNullOrEmpty(routePath, "routePath");

            RouteId = routeId;
            ParentRouteId = parentRouteId;
            Title = title;
            Icon = icon;
            RoutePath = routePath;
            SortOrder = sortOrder;
        }

        /// <summary>
        /// Gets the route id.
        /// </summary>
        public string RouteId { get; private set; }

        /// <summary>
        /// Gets the parent route id.
        /// </summary>
        public string ParentRouteId { get; private set; }

        /// <summary>
        /// Gets the name of the report
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Gets the report back office view route path
        /// </summary>
        public string RoutePath { get; private set; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public int SortOrder { get; private set; }
    }
}