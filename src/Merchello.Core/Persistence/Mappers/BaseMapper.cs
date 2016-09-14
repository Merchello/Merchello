namespace Merchello.Core.Persistence.Mappers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    using Merchello.Core.Acquired;

    using NPoco;

    internal abstract class BaseMapper
    {

        internal abstract ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache { get; }


        internal string Map(string propertyName)
        {
            DtoMapModel dtoTypeProperty;
            return PropertyInfoCache.TryGetValue(propertyName, out dtoTypeProperty)
                ? GetColumnName(dtoTypeProperty.Type, dtoTypeProperty.PropertyInfo)
                : string.Empty;
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

        internal virtual string GetColumnName(Type dtoType, PropertyInfo dtoProperty)
        {
            var tableNameAttribute = dtoType.FirstAttribute<TableNameAttribute>();
            string tableName = tableNameAttribute.Value;

            var columnAttribute = dtoProperty.FirstAttribute<ColumnAttribute>();
            string columnName = columnAttribute.Name;

            // TODO
            //string columnMap = string.Format("{0}.{1}",
            //                                 SqlSyntaxContext.SqlSyntaxProvider.GetQuotedTableName(tableName),
            //                                 SqlSyntaxContext.SqlSyntaxProvider.GetQuotedColumnName(columnName));
            //return columnMap;
            return string.Empty;
        }
    }
}
