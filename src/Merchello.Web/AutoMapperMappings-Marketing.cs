namespace Merchello.Web
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.MapperResolvers;
    using Merchello.Web.Models.MapperResolvers.Offers;

    /// <summary>
    /// The auto mapper mappings.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Creates marketing mappings.
        /// </summary>
        private static void CreateMarketingMappings()
        {
            // Offer
            AutoMapper.Mapper.CreateMap<IOfferSettings, OfferSettingsDisplay>()
                .ForMember(
                    dest => dest.OfferExpires,
                    opt =>
                    opt.ResolveUsing<OfferSettingsOfferExpiresResolver>()
                        .ConstructedBy(() => new OfferSettingsOfferExpiresResolver()))
                .ForMember(
                    dest => dest.ComponentDefinitions,
                    opt =>
                    opt.ResolveUsing<OfferSettingsComponentDefinitionsValueResolver>()
                        .ConstructedBy(() => new OfferSettingsComponentDefinitionsValueResolver()));

            AutoMapper.Mapper.CreateMap<OfferComponentBase, OfferComponentDefinitionDisplay>()
                .ForMember(
                    dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<OfferComponentExtendedDataResolver>().ConstructedBy(() => new OfferComponentExtendedDataResolver()))
                .ForMember(
                    dest => dest.Name,
                    opt =>
                    opt.ResolveUsing<OfferComponentAttributeValueResolver>()
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("name")))
                .ForMember(
                    dest => dest.Description,
                    opt =>
                    opt.ResolveUsing<OfferComponentAttributeValueResolver>()
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("description")))
                .ForMember(
                    dest => dest.ComponentKey,
                    opt =>
                    opt.ResolveUsing<OfferComponentAttributeValueResolver>()
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("key")))
                .ForMember(
                    dest => dest.DialogEditorView,
                    opt =>
                    opt.ResolveUsing<OfferComponentAttributeValueResolver>()
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("editorView")))
                .ForMember(
                    dest => dest.RestrictToType,
                    opt =>
                    opt.ResolveUsing<OfferComponentAttributeValueResolver>()
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("restrictToType")))
                 .ForMember(
                    dest => dest.TypeGrouping,
                    opt =>
                    opt.ResolveUsing<OfferComponentTypeGroupingResolver>()
                       .ConstructedBy(() => new OfferComponentTypeGroupingResolver()));

            AutoMapper.Mapper.CreateMap<IOfferProvider, OfferProviderDisplay>()
                .ForMember(
                    dest => dest.BackOfficeTree,
                    opt =>
                    opt.ResolveUsing<OfferProviderBackOfficeAttributeValueResolver>()
                        .ConstructedBy(() => new OfferProviderBackOfficeAttributeValueResolver()));

        }
    }
}