namespace Merchello.Web.Reporting
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a report resolver.
    /// </summary>
    internal interface IReportApiControllerResolver
    {
        /// <summary>
        /// Gets a collection of all <see cref="ReportController"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="ReportController"/>s
        /// </returns>
        IEnumerable<ReportController> GetAll();
    }
}