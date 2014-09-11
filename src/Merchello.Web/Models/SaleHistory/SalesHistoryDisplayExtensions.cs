namespace Merchello.Web.Models.SaleHistory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The sales history display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class SalesHistoryDisplayExtensions
    {
        /// <summary>
        /// Maps an <see cref="IEnumerable{AuditDisplay}"/> to a <see cref="SalesHistoryDisplay"/>
        /// </summary>
        /// <param name="auditLogs">
        /// The audit logs.
        /// </param>
        /// <returns>
        /// The <see cref="SalesHistoryDisplay"/>.
        /// </returns>
        public static SalesHistoryDisplay ToSalesHistoryDisplay(this IEnumerable<AuditLogDisplay> auditLogs)
        {
            var logsArray = auditLogs.ToArray().OrderBy(x => x.RecordDate);
            var history = new SalesHistoryDisplay();
            if (!logsArray.Any()) return history;

            var dates = logsArray.Select(x => x.RecordDate.GetDayDate()).Distinct();

            history.DailyLogs = dates.Select(logsArray.BuildDailyAuditLogDisplay).OrderByDescending(x => x.Day);

            return history;
        }

        /// <summary>
        /// Builds the <see cref="DailyAuditLogDisplay"/>.
        /// </summary>
        /// <param name="auditLogs">
        /// The audit logs.
        /// </param>
        /// <param name="day">
        /// The day.
        /// </param>
        /// <returns>
        /// The <see cref="DailyAuditLogDisplay"/>.
        /// </returns>
        private static DailyAuditLogDisplay BuildDailyAuditLogDisplay(this IEnumerable<AuditLogDisplay> auditLogs, DateTime day)
        {
            return new DailyAuditLogDisplay()
            {
                Day = day,
                Logs =
                    auditLogs.ToArray()
                        .OrderByDescending(x => x.RecordDate)
                        .Where(y => y.RecordDate.GetDayDate() == day)
            };
        }

        /// <summary>
        /// Gets a day representation of the record
        /// </summary>
        /// <param name="recordDate">
        /// The record date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime GetDayDate(this DateTime recordDate)
        {
            return DateTime.Parse(recordDate.ToShortDateString());
        }
    }
}