namespace Merchello.Core.Reporting
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a report resolver.
    /// </summary>
    public interface IReportResolver
    {
        /// <summary>
        /// Gets a collection of all <see cref="IReport"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IReport"/>s
        /// </returns>
        IEnumerable<IReport> GetAll();

        /// <summary>
        /// Gets a report by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report alias
        /// </param>
        /// <returns>
        /// The <see cref="IReport"/>.
        /// </returns>
        IReportDataAggregator GetByAlias(string alias);
    }
}