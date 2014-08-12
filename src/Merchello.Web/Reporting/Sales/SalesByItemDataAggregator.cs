namespace Merchello.Web.Reporting.Sales
{
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Reporting;

    [ReportDataAggregator("salesByItem", "Sales by item", "Responsible for aggregating sales by item.")]
    public class SalesByItemDataAggregator : ReportDataAggregatorBase<IEnumerable<IProductVariant>, SalesByItemQuerySettings>
    {
        public override IEnumerable<IProductVariant> GetReportData(SalesByItemQuerySettings querySettings)
        {
            throw new System.NotImplementedException();
        }
    }
}