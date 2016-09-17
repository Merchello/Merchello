namespace Merchello.Umbraco.Mapping
{
    using AutoMapper;

    using Merchello.Core.Acquired.Persistence.DatabaseModelDefinitions;
    using Merchello.Core.Mapping;
    using Merchello.Umbraco.Adapters;

    /// <summary>
    /// Represents AutoMapper mapping for Merchello versions of Umbraco class to Umbraco classes.
    /// </summary>
    public class UmbracoNativeMappings : MerchelloMapperConfiguration
    {
        /// <inheritdoc/>
        public override void ConfigureMappings(IMapperConfiguration config)
        {
            config.CreateMap<ColumnDefinition, global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ColumnDefinition>()
                .ForMember(def => def.ModificationType, expression => expression.MapFrom(def => Converter.Convert(def.ModificationType)));

            config.CreateMap<ForeignKeyDefinition, global::Umbraco.Core.Persistence.DatabaseModelDefinitions.ForeignKeyDefinition>();

            config.CreateMap<IndexDefinition, global::Umbraco.Core.Persistence.DatabaseModelDefinitions.IndexDefinition>();

            config.CreateMap<TableDefinition, global::Umbraco.Core.Persistence.DatabaseModelDefinitions.TableDefinition>();
        }
    }
}