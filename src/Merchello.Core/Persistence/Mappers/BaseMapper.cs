namespace Merchello.Core.Persistence.Mappers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    using Merchello.Core.Acquired;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// Represents a the base mapper used to help build queries in SQL specific syntaxes.
    /// </summary>
    /// <remarks>
    /// This is essentially the same class as Umbraco's class with the same name only the reference types are Merchello's
    /// for Merchello specific mappings.
    /// </remarks>
    /// <seealso cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v8/src/Umbraco.Core/Persistence/Mappers/BaseMapper.cs"/>
    public abstract class BaseMapper
    {
        /// <summary>
        /// Gets the thread safe dictionary of property info used.
        /// </summary>
        internal abstract ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache { get; }

        /// <summary>
        /// Maps an entity property to a column in the database.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The Sql Syntax Provider.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="throws">
        /// If true, this method will through an exception if the property was not found.
        /// </param>
        /// <returns>
        /// The mapped column.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Conditionally throws an exception if the mapping fails
        /// </exception>
        internal string Map(ISqlSyntaxProviderAdapter sqlSyntax, string propertyName, bool throws = false)
        {
            DtoMapModel dtoTypeProperty;

            if (PropertyInfoCache.TryGetValue(propertyName, out dtoTypeProperty))
                return GetColumnName(sqlSyntax, dtoTypeProperty.Type, dtoTypeProperty.PropertyInfo);

            if (throws)
                throw new InvalidOperationException("Could not get the value with the key " + propertyName + " from the property info cache, keys available: " + string.Join(", ", PropertyInfoCache.Keys));

            return string.Empty;
        }

        /// <summary>
        /// Adds a mapping to the concurrent cache dictionary.
        /// </summary>
        /// <param name="sourceMember">
        /// The entity side of the relation.
        /// </param>
        /// <param name="destinationMember">
        /// The DTO side of the relation.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the entity (source)
        /// </typeparam>
        /// <typeparam name="TDestination">
        /// The type of the DTO (destination)
        /// </typeparam>
        internal void CacheMap<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember)
        {
            var property = ResolveMapping(sourceMember, destinationMember);
            PropertyInfoCache.AddOrUpdate(property.SourcePropertyName, property, (x, y) => property);
        }

        /// <summary>
        /// Resolves the mapping for the specific property.
        /// </summary>
        /// <param name="sourceMember">
        /// The source member.
        /// </param>
        /// <param name="destinationMember">
        /// The destination member.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the source
        /// </typeparam>
        /// <typeparam name="TDestination">
        /// The type of the destination
        /// </typeparam>
        /// <returns>
        /// The <see cref="DtoMapModel"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Throws an exception if the resolution fails
        /// </exception>
        internal DtoMapModel ResolveMapping<TSource, TDestination>(Expression<Func<TSource, object>> sourceMember, Expression<Func<TDestination, object>> destinationMember)
        {
            var source = ExpressionHelper.FindProperty(sourceMember);
            var destination = (PropertyInfo)ExpressionHelper.FindProperty(destinationMember);

            if (destination == null)
            {
                throw new InvalidOperationException("The 'destination' returned was null, cannot resolve the mapping");
            }

            return new DtoMapModel(typeof(TDestination), destination, source.Name);
        }

        /// <summary>
        /// Gets the database column name.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The sql syntax provider used to format the column name string.
        /// </param>
        /// <param name="dtoType">
        /// The type of the DTO.
        /// </param>
        /// <param name="dtoProperty">
        /// The DTO property to get respective column name.
        /// </param>
        /// <returns>
        /// The Sql specific formatted column name.
        /// </returns>
        internal virtual string GetColumnName(ISqlSyntaxProviderAdapter sqlSyntax, Type dtoType, PropertyInfo dtoProperty)
        {
            var tableNameAttribute = dtoType.FirstAttribute<TableNameAttribute>();
            var tableName = tableNameAttribute.Value;

            var columnAttribute = dtoProperty.FirstAttribute<ColumnAttribute>();
            var columnName = columnAttribute.Name;

            var columnMap = sqlSyntax.GetQuotedTableName(tableName) + "." + sqlSyntax.GetQuotedColumnName(columnName);
            return columnMap;
        }

        /// <summary>
        /// Performs the actual work of building the cached mapping.
        /// </summary>
        protected abstract void BuildMap();

        /// <summary>
        /// Executes the building of the cache maps.
        /// </summary>
        private void Build()
        {
            BuildMap();
        }
    }
}
