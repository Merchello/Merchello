namespace Merchello.Core.Configuration.BackOffice
{
    using System.Collections.Generic;

    using Merchello.Core.EntityCollections;

    /// <inheritdoc/>
    internal class DashboardTreeNode : IDashboardTreeNode
    {
        /// <inheritdoc/>
        public string Icon { get; set; }

        /// <inheritdoc/>
        public string ParentRouteId { get; set; }

        /// <inheritdoc/>
        public string RouteId { get; set; }

        /// <inheritdoc/>
        public string RoutePath { get; set; }

        /// <inheritdoc/>
        public int SortOrder { get; set; }

        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public bool Visible { get; set; }

        /// <inheritdoc/>
        public bool SelfManagedProvidersBeforeStaticProviders { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IDashboardTreeNodeKeyLink> SelfManagedEntityCollectionProviders { get; set; }
    }
}