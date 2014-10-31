namespace Merchello.Web
{
    using Core.Gateways.Shipping;
    using Core.Gateways.Shipping.FixedRate;
    using Core.Models;
    using Core.Models.Interfaces;
    using Models.ContentEditing;
    using Models.MapperResolvers;

    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Binds the shipping related mappings
        /// </summary>
        private static void CreateShippingMappings()
        {
            // shipping     
            AutoMapper.Mapper.CreateMap<IShippingGatewayProvider, ShippingGatewayProviderDisplay>();
                ////.ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()));

            AutoMapper.Mapper.CreateMap<IShipCountry, ShipCountryDisplay>();

            AutoMapper.Mapper.CreateMap<IShipMethod, ShipMethodDisplay>();

            AutoMapper.Mapper.CreateMap<IShippingGatewayMethod, ShipMethodDisplay>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.ShipMethod.Key))
                .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.ShipMethod.ProviderKey))
                .ForMember(dest => dest.ShipCountryKey, opt => opt.MapFrom(src => src.ShipMethod.ShipCountryKey))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ShipMethod.Name))
                .ForMember(dest => dest.Provinces, opt => opt.MapFrom(src => src.ShipMethod.Provinces))
                .ForMember(dest => dest.ServiceCode, opt => opt.MapFrom(src => src.ShipMethod.ServiceCode))
                .ForMember(dest => dest.Surcharge, opt => opt.MapFrom(src => src.ShipMethod.Surcharge))
                .ForMember(dest => dest.Taxable, opt => opt.MapFrom(src => src.ShipMethod.Taxable))
                .ForMember(dest => dest.DialogEditorView, opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>().ConstructedBy(() => new GatewayMethodDialogEditorViewResolver()));

            AutoMapper.Mapper.CreateMap<IShipProvince, ShipProvinceDisplay>();
            AutoMapper.Mapper.CreateMap<IShippingFixedRateTable, ShipFixedRateTableDisplay>();
            AutoMapper.Mapper.CreateMap<IShipRateTier, ShipRateTierDisplay>();

            // Shipment
            AutoMapper.Mapper.CreateMap<IShipmentStatus, ShipmentStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IShipment, ShipmentDisplay>();
        }
    }
}