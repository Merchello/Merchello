namespace Merchello.Web.Reporting
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a report resolver.
    /// </summary>
    public interface IReportControllerResolver
    {
        /// <summary>
        /// Gets a collection of all <see cref="IReportApiController"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IReportApiController"/>s
        /// </returns>
        IEnumerable<IReportApiController> GetAll();

        /// <summary>
        /// Gets a report by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report alias
        /// </param>
        /// <returns>
        /// The <see cref="IReportApiController"/>.
        /// </returns>
        IReportApiController GetByAlias(string alias);
    }
}