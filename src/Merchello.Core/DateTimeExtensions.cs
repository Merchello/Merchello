﻿namespace Merchello.Core
{
    using System;
    using System.Globalization;
    using System.Runtime.Remoting.Messaging;

    /// <summary>
    /// Extension methods for <see cref="DateTime"/>
    /// </summary>
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// The SQL date time min value string.
        /// </summary>
        private const string SqlDateTimeMinValueString = "1753-1-1";

        /// <summary>
        /// The SQL date time max value string.
        /// </summary>
        private const string SqlDateTimeMaxValueString = "9999-12-31";

        /// <summary>
        /// Converts a date time min value to null.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The nullable <see cref="DateTime"/>.
        /// </returns>
        public static DateTime? ConverDateTimeMinValueToNull(this DateTime value)
        {
            return !value.Equals(DateTime.MinValue) ? value : (DateTime?)null;
        }

        /// <summary>
        /// The convert date time max value to null.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The nullable <see cref="DateTime"/>.
        /// </returns>
        public static DateTime? ConvertDateTimeMaxValueToNull(this DateTime value)
        {
            return !value.Equals(DateTime.MaxValue) ? value : (DateTime?)null;
        }

        /// <summary>
        /// The convert date time null to min value.
        /// </summary>
        /// <param name="dt">
        /// The date time value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ConvertDateTimeNullToMinValue(this DateTime? dt)
        {
            return dt == null ? DateTime.MinValue : dt.Value;
        }

        /// <summary>
        /// The convert date time null to max value.
        /// </summary>
        /// <param name="dt">
        /// The date time value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ConvertDateTimeNullToMaxValue(this DateTime? dt)
        {
            return dt == null ? DateTime.MaxValue : dt.Value;
        }

        /// <summary>
        /// Checks if parameter is a min SQL DateTime value and return the .NET DateTime Min.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime SqlDateTimeMinValueAsDateTimeMinValue(this DateTime value)
        {
            return !value.Equals(SqlDateTimeMinValue()) ? value : DateTime.MinValue;
        }

        /// <summary>
        /// Checks if parameter is a max SQL DateTime value and return the .NET DateTime Max.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime SqlDateTimeMaxValueAsSqlDateTimeMaxValue(this DateTime value)
        {
            return !value.Equals(SqlDateTimeMaxValue()) ? value : DateTime.MaxValue;
        }

        /// <summary>
        /// If value parameter passes is DateTime min returns the SQL Server min date time value
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime AsSqlDateTimeMinValue(this DateTime value)
        {
            return !value.Equals(DateTime.MinValue) ? value : SqlDateTimeMinValue();
        }

        /// <summary>
        /// If value parameter passes is DateTime max returns the SQL Server max date time value
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime AsSqlDateTimeMaxValue(this DateTime value)
        {
            return !value.Equals(DateTime.MinValue) ? value : SqlDateTimeMaxValue();
        }

        /// <summary>
        /// Gets the first day a month month.
        /// </summary>
        /// <param name="current">
        /// The reference date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime FirstOfMonth(this DateTime current)
        {
            return new DateTime(current.Year, current.Month, 1);
        }

        /// <summary>
        /// Gets the last day of a month.
        /// </summary>
        /// <param name="current">
        /// The reference date.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime EndOfMonth(this DateTime current)
        {
            return new DateTime(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month));
        }

        /// <summary>
        /// Gets the start of week.
        /// </summary>
        /// <param name="dt">
        /// The date time.
        /// </param>
        /// <param name="startOfWeek">
        /// The start of week.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }


        /// <summary>
        /// Parses the SQL DateTime min value string
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime SqlDateTimeMinValue()
        {
            return DateTime.Parse(SqlDateTimeMinValueString);
        }

        /// <summary>
        /// Parses the SQL DateTime max value string
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime SqlDateTimeMaxValue()
        {
            return DateTime.Parse(SqlDateTimeMaxValueString);
        }
    }
}