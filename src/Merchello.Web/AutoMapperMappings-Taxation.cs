using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.Models.MapperResolvers;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal  static partial class AutoMapperMappings
    {
        private static void BindTaxationMappings()
        {
            // taxation
            AutoMapper.Mapper.CreateMap<ITaxMethod, TaxMethodDisplay>();

            AutoMapper.Mapper.CreateMap<ITaxMethod, TaxProvinceDisplay>();


            AutoMapper.Mapper.CreateMap<ITaxationGatewayMethod, TaxMethodDisplay>()
                .ForMember(dest => dest.Key,
                    opt => opt.MapFrom(src => src.TaxMethod.Key)
                )
                .ForMember(dest => dest.ProviderKey,
                    opt => opt.MapFrom(src => src.TaxMethod.ProviderKey)
                )
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.TaxMethod.Name)
                )
                .ForMember(dest => dest.Provinces,
                    opt => opt.MapFrom(src => src.TaxMethod.Provinces)
                )
                .ForMember(dest => dest.CountryCode,
                    opt => opt.MapFrom(src => src.TaxMethod.CountryCode)
                )
                .ForMember(dest => dest.PercentageTaxRate,
                    opt => opt.MapFrom(src => src.TaxMethod.PercentageTaxRate)
                )
                .ForMember(dest => dest.DialogEditorView,
                    opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>()
                        .ConstructedBy(() => new GatewayMethodDialogEditorViewResolver()));

            AutoMapper.Mapper.CreateMap<ITaxProvince, TaxProvinceDisplay>();
        }
    }
}