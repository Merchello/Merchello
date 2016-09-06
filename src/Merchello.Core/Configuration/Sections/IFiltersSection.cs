namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a configuration section for configurations related to Merchello entity "filters".
    /// </summary>
    public interface IFiltersSection
    {
        /// <summary>
        /// Gets a collection of <see cref="IEnitityCollectionProvider"/>s that can be used by product filters
        /// </summary>
        /// REFACTOR-todo change this object to the actual result
        IEnumerable<object> ProductFilterProviders { get; }
    }
}