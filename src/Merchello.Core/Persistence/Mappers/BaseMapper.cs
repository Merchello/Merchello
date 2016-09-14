namespace Merchello.Core.Persistence.Mappers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    using Merchello.Core.Acquired;
    using Merchello.Core.Acquired.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// Represents a the base mapper used to help build queries in SQL specific syntaxes.
    /// </summary>
    /// <remarks>
    /// This is essentially the same class as Umbraco's class with the same name only the reference types are Merchello's
    /// for Merchello specific mappings.
    /// </remarks>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v8/src/Umbraco.Core/Persistence/Mappers/BaseMapper.cs"/>
    internal abstract class BaseMapper
    {
        /// <summary>
        /// Gets the thread safe dictionary of property info used.
        /// </summary>
        internal abstract ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache { get; }


        internal string Map(ISqlSyntaxProvider sqlSyntax, string propertyName, bool throws = false)
        {
            DtoMapModel dtoTypeProperty;

            if (PropertyInfoCache.TryGetValue(propertyName, out dtoTypeProperty))
                return GetColumnName(sqlSyntax, dtoTypeProperty.Type, dtoTypeProperty.PropertyInfo);

            if (throws)
                throw new InvalidOperationException("Could not get the value with the key " + propertyName + " from the property info cache, keys available: " + string.Join(", ", PropertyInfoCache.Keys));

            return string.Empty;
        }

        internal void CacheMap<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember)
        {
            var property = ResolveMapping(sourceMember, destinationMember);
            PropertyInfoCache.AddOrUpdate(property.SourcePropertyName, property, (x, y) => property);
        }

        internal DtoMapModel ResolveMapping<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember)
        {
            var source = ExpressionHelper.FindProperty(sourceMember);
            var destination = ExpressionHelper.FindProperty(destinationMember) as PropertyInfo;

            return new DtoMapModel(typeof(TDestination), destination, source.Name);
        }

        internal virtual string GetColumnName(ISqlSyntaxProvider sqlSyntax, Type dtoType, PropertyInfo dtoProperty)
        {
            var tableNameAttribute = dtoType.FirstAttribute<TableNameAttribute>();
            string tableName = tableNameAttribute.Value;

            var columnAttribute = dtoProperty.FirstAttribute<ColumnAttribute>();
            var columnName = columnAttribute.Name;

            var columnMap = sqlSyntax.GetQuotedTableName(tableName) + "." + sqlSyntax.GetQuotedColumnName(columnName);
            return columnMap;
        }
    }
}
