namespace Merchello.Core.Reporting
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the ReportDataAggregatorResolver.
    /// </summary>
    public interface IReportDataAggregatorResolver
    {
        /// <summary>
        /// Gets a collection of all <see cref="object"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="object"/>s
        /// </returns>
        IEnumerable<object> GetAll();

        /// <summary>
        /// Gets a report data aggregator by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report aggregator alias
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object GetByAlias(string alias);
    }
}