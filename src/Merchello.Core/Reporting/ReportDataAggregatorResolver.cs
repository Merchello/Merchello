namespace Merchello.Core.Reporting
{
    using System;
    using System.Collections.Generic;
    using ObjectResolution;

    /// <summary>
    /// Represents a report data aggregator resolver.
    /// </summary>
    internal class ReportDataAggregatorResolver : MerchelloManyObjectsResolverBase<ReportDataAggregatorResolver, IReportDataAggregator>,  IReportDataAggregatorResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportDataAggregatorResolver"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public ReportDataAggregatorResolver(IEnumerable<Type> value) 
            : base(value)
        {
        }

        /// <summary>
        /// Gets a collection of instantiated <see cref="IReportDataAggregator"/>s
        /// </summary>
        protected override IEnumerable<IReportDataAggregator> Values
        {
            get
            {
                using (GetWriteLock())
                {
                }

                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="IReportDataAggregator"/>s
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IReportDataAggregator"/>s
        /// </returns>
        public IEnumerable<IReportDataAggregator> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a report data aggregator by it's key defined in the attribute
        /// </summary>
        /// <param name="alias">
        /// The report alias
        /// </param>
        /// <returns>
        /// The <see cref="IReportDataAggregator"/>.
        /// </returns>
        public IReportDataAggregator GetByAlias(string alias)
        {
            throw new System.NotImplementedException();
        }
    }
}