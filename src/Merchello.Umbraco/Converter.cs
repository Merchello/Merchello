namespace Merchello.Umbraco
{
    using AutoMapper;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;
    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Acquired.Persistence.Querying;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents a converter for Umbraco Core value and type conversions.
    /// </summary>
    internal static class Converter
    {
        #region Database Definitions and Annotations

        /// <summary>
        /// Converts Merchello's TableDefinition to Umbraco's table definition.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The converted <see>
        ///         <cref>global::Umbraco.Core.Persistence.DatabaseModelDefinitions.TableDefinition</cref>
        ///     </see>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseModelDefinitions.TableDefinition Convert(TableDefinition table)
        {
            return Mapper.Map<global::Umbraco.Core.Persistence.DatabaseModelDefinitions.TableDefinition>(table);
        }

        /// <summary>
        /// Converts Merchello's ForeignKeyDefinition to Umbraco's ForeignKeyDefinition.
        /// </summary>
        /// <param name="foreignKey">
        /// The foreign key.
        /// </param>
        /// <returns>
        /// The converted <see>
        ///         <cref>global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ForeignKeyDefinition</cref>
        ///     </see>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ForeignKeyDefinition Convert(ForeignKeyDefinition foreignKey)
        {
            return Mapper.Map<global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ForeignKeyDefinition>(foreignKey);
        }

        /// <summary>
        /// Converts Merchello's IndexDefinition to Umbraco's IndexDefinition.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The converted <see>
        ///         <cref>global::Umbraco.Core.Persistence.DatabaseModelDefinitions.IndexDefinition</cref>
        ///     </see>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseModelDefinitions.IndexDefinition Convert(IndexDefinition index)
        {
            return Mapper.Map<global::Umbraco.Core.Persistence.DatabaseModelDefinitions.IndexDefinition>(index);
        }

        /// <summary>
        /// Converts Merchello's ColumnDefinition to Umbraco's ColumnDefinition.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <returns>
        /// The converted <see>
        ///         <cref>global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ColumnDefinition</cref>
        ///     </see>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ColumnDefinition Convert(ColumnDefinition column)
        {
            return Mapper.Map<global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ColumnDefinition>(column);
        }

        /// <summary>
        /// Converts Umbraco's ColumnInfo to Merchello's ColumnInfo.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <returns>
        /// The <see cref="ColumnInfo"/>.
        /// </returns>
        public static ColumnInfo Convert(global::Umbraco.Core.Persistence.SqlSyntax.ColumnInfo info)
        {
            return Mapper.Map<ColumnInfo>(info);
        }

        #endregion

        #region enums

        /// <summary>
        /// Converts Merchello's TextColumnType to Umbraco's TextColumnType.
        /// </summary>
        /// <param name="textColumnType">
        /// The text colum type.
        /// </param>
        /// <returns>
        /// The <see cref="TextColumnType"/>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.Querying.TextColumnType Convert(TextColumnType textColumnType)
        {
            return textColumnType == TextColumnType.NText
                       ? global::Umbraco.Core.Persistence.Querying.TextColumnType.NText
                       : global::Umbraco.Core.Persistence.Querying.TextColumnType.NVarchar;
        }

        /// <summary>
        /// Converts Merchello's IndexTypes to Umbraco's IndexTypes.
        /// </summary>
        /// <param name="indexTypes">
        /// The index types.
        /// </param>
        /// <returns>
        /// The <see cref="IndexTypes"/>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseAnnotations.IndexTypes Convert(IndexTypes indexTypes)
        {
            switch (indexTypes)
            {
                case IndexTypes.Clustered:
                    return global::Umbraco.Core.Persistence.DatabaseAnnotations.IndexTypes.Clustered;

                case IndexTypes.UniqueNonClustered:
                    return global::Umbraco.Core.Persistence.DatabaseAnnotations.IndexTypes.UniqueNonClustered;

                case IndexTypes.NonClustered:
                default:
                    return global::Umbraco.Core.Persistence.DatabaseAnnotations.IndexTypes.NonClustered;
            }
        }

        /// <summary>
        /// Converts Merchello's SpecialDbTypes to Umbraco's SpecialDbTypes.
        /// </summary>
        /// <param name="specialDbTypes">
        /// The <see cref="SpecialDbTypes"/>.
        /// </param>
        /// <returns>
        /// The converted <see cref="SpecialDbTypes"/>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseAnnotations.SpecialDbTypes Convert(SpecialDbTypes specialDbTypes)
        {
            return specialDbTypes == SpecialDbTypes.NCHAR
                       ? global::Umbraco.Core.Persistence.DatabaseAnnotations.SpecialDbTypes.NCHAR
                       : global::Umbraco.Core.Persistence.DatabaseAnnotations.SpecialDbTypes.NTEXT;
        }

        /// <summary>
        /// Converts Merchello's ModificationType to Umbraco's ModificationType.
        /// </summary>
        /// <param name="modificationType">
        /// The <see cref="ModificationType"/>.
        /// </param>
        /// <returns>
        /// The converted <see cref="ModificationType"/>.
        /// </returns>
        public static global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType Convert(ModificationType modificationType)
        {
            switch (modificationType)
            {
                case ModificationType.Create:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Create;
                case ModificationType.Delete:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Delete;
                case ModificationType.Drop:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Drop;
                case ModificationType.Insert:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Insert;
                case ModificationType.Rename:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Rename;
                case ModificationType.Update:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Update;
                case ModificationType.Alter:
                default:
                    return global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ModificationType.Alter;
            }
        }

        #endregion
    }
}