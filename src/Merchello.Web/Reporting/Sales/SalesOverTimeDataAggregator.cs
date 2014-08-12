namespace Merchello.Web.Reporting.Sales
{
    using System.Collections.Generic;

    using Core.Reporting;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The sales over time data aggregator.
    /// </summary>
    [ReportDataAggregator("salesOverTime", "Sales over time data", "Responsible for aggregating sales over time.")]
    public sealed class SalesOverTimeDataAggregator : ReportDataAggregatorBase<IEnumerable<InvoiceDisplay>, SalesOverTimeQuerySettings>
    {
       
        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="querySettings">
        /// The query Settings.
        /// </param>
        /// <returns>
        /// A collection of invoices.
        /// </returns>
        public override IEnumerable<InvoiceDisplay> GetReportData(SalesOverTimeQuerySettings querySettings)
        {
            throw new System.NotImplementedException();
        }
    }
}
