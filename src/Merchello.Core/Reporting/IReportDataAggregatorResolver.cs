namespace Merchello.Core.Reporting
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the ReportDataAggregatorResolver.
    /// </summary>
    public interface IReportDataAggregatorResolver
    {
        /// <summary>
        /// Gets a collection of all <see cref="IReportDataAggregator"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IReportDataAggregator"/>s
        /// </returns>
        IEnumerable<IReportDataAggregator> GetAll();

        /// <summary>
        /// Gets a report data aggregator by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report alias
        /// </param>
        /// <returns>
        /// The <see cref="IReportDataAggregator"/>.
        /// </returns>
        IReportDataAggregator GetByAlias(string alias);
    }
}