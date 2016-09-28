namespace Merchello.Core.Models.Rdbms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NPoco;

    /// <summary>
    /// Extension methods for <see cref="IDto"/> POCOs.
    /// </summary>
    internal static class DtoExtensions
    {
        /// <summary>
        /// Gets a pair of values indicating the database table name and the count of the columns in that table.
        /// </summary>
        /// <param name="dto">
        /// The <see cref="IDto"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Tuple{String, Int}" /> [table name | column count].
        /// </returns>
        public static Tuple<string, int> TableNameColumnCount(this IDto dto)
        {
            var tableName = ((TableNameAttribute)dto.GetType().GetCustomAttribute(typeof(TableNameAttribute), false)).Value;
            var columnCount = dto.GetColumnProperties().Count();

            return new Tuple<string, int>(tableName, columnCount);
        }

        /// <summary>
        /// Gets the values of a column in the table.
        /// </summary>
        /// <param name="dto">
        /// The <see cref="IDto"/> representing the table.
        /// </param>
        /// <returns>
        /// The collection of column values represented as a <see cref="Tuple{String, Object}"/> [ column name | value ]
        /// </returns>
        internal static IEnumerable<Tuple<string, object>> ColumnValues(this IDto dto)
        {
            var properties = dto.GetColumnProperties();

            return properties.Select(p => 
                    new Tuple<string, object>(
                        p.GetCustomAttribute<ColumnAttribute>().Name, 
                        p.GetValue(dto, null)));
        }

        /// <summary>
        /// Gets the properties of a database column.
        /// </summary>
        /// <param name="dto">
        /// The <see cref="IDto"/> corresponding to the table.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PropertyInfo}"/>.
        /// </returns>
        internal static IEnumerable<PropertyInfo> GetColumnProperties(this IDto dto)
        {
            return dto.GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(ColumnAttribute), false) && x.GetCustomAttribute<ColumnAttribute>().GetType() != typeof(ResultColumnAttribute));
        }
    }
}