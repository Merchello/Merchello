namespace Merchello.Web.Editors.Reports
{
    using Merchello.Core;
    using System;
    using System.Collections.Generic;
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
            /// <param name="invoiceStatusKeys">
            /// The invoice status keys
            /// </param>
            /// <returns>
            /// The <see cref="Sql"/>.
            /// </returns>
            public static Sql GetSkuSaleCountSql(DateTime startDate, DateTime endDate, Guid typeFieldKey, int count, IEnumerable<Guid> invoiceStatusKeys)
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
                                    AND MI.invoiceStatusKey IN (@invoiceStatusKeys)
				                    GROUP BY sku)  T3 ON T2.sku = T3.sku
		                    WHERE T2.lineItemTfKey = @tfKey
	                    ) Q1
                    INNER JOIN (
	                    SELECT	Sku,
                            SUM(quantity) as quantitySold
		                    FROM	merchInvoiceItem MII
                            INNER JOIN merchInvoice MI ON MII.invoiceKey = MI.pk
				            AND	MI.invoiceDate BETWEEN @start AND @end
                            AND MI.invoiceStatusKey IN (@invoiceStatusKeys)
		                    GROUP BY sku
                    ) Q2 ON Q1.sku = Q2.sku
                    WHERE Q2.quantitySold > 0
                    ORDER BY Q2.quantitySold DESC";

                return new Sql(sql, new { @top = count, @start = startDate, @end = endDate, @tfKey = typeFieldKey, @invoiceStatusKeys = invoiceStatusKeys });
            }

            public static Sql GetSaleSearchSql(DateTime startDate, DateTime endDate, IEnumerable<Guid> invoiceStatuses, string search, string typeFieldKey = "D462C051-07F4-45F5-AAD2-D5C844159F04")
            {
                var sql = new Sql(@"SELECT merchInvoiceItem.[name], merchInvoiceItem.quantity, merchInvoiceItem.price, merchInvoiceItem.extendedData
                         FROM merchInvoiceItem INNER JOIN
                         merchInvoice ON merchInvoiceItem.invoiceKey = merchInvoice.pk INNER JOIN
                         merchInvoiceStatus ON merchInvoice.invoiceStatusKey = merchInvoiceStatus.pk")
                         .Append("WHERE merchInvoiceStatus.pk IN (@invoiceStatuses)", new { invoiceStatuses })
                         .Append("AND lineItemTfKey = @typeFieldKey", new { @typeFieldKey = Guid.Parse(typeFieldKey) })
                         .Append("AND merchInvoice.invoiceDate BETWEEN @startDate AND @endDate", new { @startDate = startDate.GetStartOfDay(), @endDate = endDate.GetEndOfDay() });


                if (!string.IsNullOrWhiteSpace(search))
                {
                    sql.Append("AND merchInvoiceItem.[name] LIKE @search", new { @search = string.Format("%{0}%", search) });
                }


       //         //merchInvoiceItem.sku, merchInvoiceItem.[name], 
       //         var raw = @"SELECT merchInvoiceItem.quantity, merchInvoiceItem.price, merchInvoiceItem.extendedData
       //                  FROM merchInvoiceItem INNER JOIN
       //                  merchInvoice ON merchInvoiceItem.invoiceKey = merchInvoice.pk INNER JOIN
       //                  merchInvoiceStatus ON merchInvoice.invoiceStatusKey = merchInvoiceStatus.pk
						 //WHERE merchInvoiceStatus.pk IN ('1F872A1A-F0DD-4C3E-80AB-99799A28606E', '6606B0EA-15B6-44AA-8557-B2D9D049645C')
						 //AND lineItemTfKey = 'D462C051-07F4-45F5-AAD2-D5C844159F04' 
						 //AND merchInvoice.invoiceDate BETWEEN '01 May 2018' AND '10 January 2019'
						 //AND merchInvoiceItem.[name] LIKE '%nene black%'";

                return sql;
            }
        }
         
    }
}