namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Configuration.BackOffice;

    /// <summary>
    /// Represents a configuration section for configurations related to Merchello entity "filters".
    /// </summary>
    public interface IFiltersSection
    {
        /// <summary>
        /// Gets a collection of <see cref="IDashboardTreeNodeKeyLink"/>s that can be used by product filters
        /// </summary>
        IEnumerable<IDashboardTreeNodeKeyLink> Products { get; }
    }
}