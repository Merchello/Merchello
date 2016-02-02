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
            /// <param name="count">
            /// The count to return
            /// </param>
            /// <returns>
            /// The <see cref="Sql"/>.
            /// </returns>
            public static Sql GetSkuSaleCountSql(DateTime startDate, DateTime endDate, Guid typeFieldKey, int count = 10)
            {
                var sql = @"SELECT TOP(@top) Q1.Sku,
                            Q1.invoiceCount,
		                    Q2.quantitySold
                    FROM (
		                    SELECT  DISTINCT(T2.[sku]),
				                    T3.invoiceCount
		                    FROM   merchInvoiceItem T2
		                    INNER JOIN (
				                    SELECT  sku, 
                                            COUNT(*) as invoiceCount
				                    FROM  merchInvoiceItem MII
				                    INNER JOIN merchInvoice MI ON MII.invoiceKey = MI.pk
				                    AND	MI.invoiceDate BETWEEN @start AND @end
				                    GROUP BY sku)  T3 ON T2.sku = T3.sku
		                    WHERE T2.lineItemTfKey = @tfKey
	                    ) Q1
                    INNER JOIN (
	                    SELECT	Sku,
                            SUM(quantity) as quantitySold
		                    FROM	merchInvoiceItem MII
                            INNER JOIN merchInvoice MI ON MII.invoiceKey = MI.pk
				            AND	MI.invoiceDate BETWEEN @start AND @end
		                    GROUP BY sku
                    ) Q2 ON Q1.sku = Q2.sku
                    WHERE Q2.quantitySold > 0
                    ORDER BY Q2.quantitySold DESC";

                return new Sql(sql, new { @top = count, @start = startDate, @end = endDate, @tfKey = typeFieldKey });
            }
        }
         
    }
}