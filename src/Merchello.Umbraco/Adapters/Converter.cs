namespace Merchello.Umbraco.Adapters
{
    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;
    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Acquired.Persistence.Querying;

    /// <summary>
    /// Represents a converter for enum value conversion.
    /// </summary>
    internal static class Converter
    {
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
    }
}