using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.Models.MapperResolvers;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {

        private static void BindShippingMappings()
        {
            // shipping     
            AutoMapper.Mapper.CreateMap<IShippingGatewayProvider, ShippingGatewayProviderDisplay>();
            AutoMapper.Mapper.CreateMap<IShipCountry, ShipCountryDisplay>();

            AutoMapper.Mapper.CreateMap<IShipMethod, ShipMethodDisplay>();

            AutoMapper.Mapper.CreateMap<IShippingGatewayMethod, ShipMethodDisplay>()
                .ForMember(dest => dest.Key,
                    opt => opt.MapFrom(src => src.ShipMethod.Key)
                )
                .ForMember(dest => dest.ProviderKey,
                    opt => opt.MapFrom(src => src.ShipMethod.ProviderKey)
                )
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.ShipMethod.Name)
                )
                .ForMember(dest => dest.Provinces,
                    opt => opt.MapFrom(src => src.ShipMethod.Provinces)
                )
                .ForMember(dest => dest.ServiceCode,
                    opt => opt.MapFrom(src => src.ShipMethod.ServiceCode)
                )
                .ForMember(dest => dest.Surcharge,
                    opt => opt.MapFrom(src => src.ShipMethod.Surcharge)
                )
                .ForMember(dest => dest.Taxable,
                    opt => opt.MapFrom(src => src.ShipMethod.Taxable)
                )
                .ForMember(dest => dest.DialogEditorView,
                    opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>()
                        .ConstructedBy(() => new GatewayMethodDialogEditorViewResolver())
                );

            AutoMapper.Mapper.CreateMap<IFixedRateShippingGatewayMethod, FixedRateShipMethodDisplay>()
                .ForMember(dest => dest.DialogEditorView,
                    opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>()
                        .ConstructedBy(() => new GatewayMethodDialogEditorViewResolver())
                );

            AutoMapper.Mapper.CreateMap<IShipProvince, ShipProvinceDisplay>();
            AutoMapper.Mapper.CreateMap<IShippingFixedRateTable, ShipFixedRateTableDisplay>();
            AutoMapper.Mapper.CreateMap<IShipRateTier, ShipRateTierDisplay>();

            // Shipment
            AutoMapper.Mapper.CreateMap<IShipment, ShipmentDisplay>();

        }
    }
}