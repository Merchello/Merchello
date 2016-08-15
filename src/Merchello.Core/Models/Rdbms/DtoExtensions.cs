namespace Merchello.Core.Models.Rdbms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The DTO extensions.
    /// </summary>
    internal static class DtoExtensions
    {
        /// <summary>
        /// The table name.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static Tuple<string, int> TableNameColumnCount(this IDto dto)
        {
            var tableName = ((TableNameAttribute)dto.GetType().GetCustomAttribute(typeof(TableNameAttribute), false)).Value;
            var columnCount = dto.GetColumnProperties().Count();

            return new Tuple<string, int>(tableName, columnCount);
        }

        /// <summary>
        /// The column values.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The collection of column tuples
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
        /// Gets the column properties.
        /// </summary>
        /// <param name="dto">
        /// The dto.
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