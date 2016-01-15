namespace Merchello.Web.Editors.Reports
{
    using System;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Utility class to keep report SQL in a single logical location.
    /// </summary>
    internal static class ReportSqlHelper
    {
        /// <summary>
        /// Queries for SaleByItem Report.
        /// </summary>
        public static class SalesByItem
        {
            /// <summary>
            /// Gets the SQL for top 5 SKUs and sale count over a date range.
            /// </summary>
            /// <param name="startDate">
            /// The start date.
            /// </param>
            /// <param name="endDate">
            /// The end date.
            /// </param>
            /// <param name="typeFieldKey">
            /// The line item type field key.  Set to "product" in API controller
            /// </param>
            /// <returns>
            /// The <see cref="Sql"/>.
            /// </returns>
            public static Sql GetSkuSaleCountSql(DateTime startDate, DateTime endDate, Guid typeFieldKey)
            {
                var sql = @"SELECT TOP(5) T1.Sku,
                               T1.saleCount
                        FROM (
		                        SELECT  DISTINCT(T2.[sku]),
				                        T3.saleCount
		                        FROM   merchInvoiceItem T2
		                        INNER JOIN (
				                        SELECT  sku, COUNT(*) as saleCount
				                        FROM  merchInvoiceItem MII
				                        INNER JOIN merchInvoice MI ON MII.invoiceKey = MI.pk
				                        AND	MI.invoiceDate BETWEEN @start AND @end
				                        GROUP BY sku)  T3 ON T2.sku = T3.sku
		                        WHERE T2.lineItemTfKey = @tfkey
	                        ) T1
                        WHERE T1.saleCount > 0
                        ORDER BY T1.saleCount DESC";

                return new Sql(sql, new { @start = startDate, @end = endDate, @tfKey = typeFieldKey });
            }
        }
         
    }
}