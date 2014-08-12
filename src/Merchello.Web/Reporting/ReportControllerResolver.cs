namespace Merchello.Web.Reporting
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an report controller resolver.
    /// </summary>
    internal class ReportControllerResolver : IReportControllerResolver
    {
        /// <summary>
        /// Gets a collection of all <see cref="IReportApiController"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IReportApiController"/>s
        /// </returns>
        public IEnumerable<IReportApiController> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a report by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report alias
        /// </param>
        /// <returns>
        /// The <see cref="IReportApiController"/>.
        /// </returns>
        public IReportApiController GetByAlias(string alias)
        {
            throw new System.NotImplementedException();
        }
    }
}