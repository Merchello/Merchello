namespace Merchello.Core.Configuration.BackOffice
{
    using System;

    /// <inheritdoc/>
    internal class DashboardTreeNodeKeyLink : IDashboardTreeNodeKeyLink
    {
        /// <inheritdoc/>
        public Guid Key { get; set; }

        /// <inheritdoc/>
        public string Icon { get; set; }

        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public bool Visible { get; set; }
    }
}