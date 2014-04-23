using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.Models.MapperResolvers;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static class AutoMapperMappings
    {
        public static void BindMappings()
        {
            // Address
            AutoMapper.Mapper.CreateMap<IAddress, AddressDisplay>();
            AutoMapper.Mapper.CreateMap<AddressDisplay, Address>();

            AutoMapper.Mapper.CreateMap<ICatalogInventory, CatalogInventoryDisplay>();
            
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();
            AutoMapper.Mapper.CreateMap<ITaxMethod, TaxProvinceDisplay>();
            
            // Invoice
            AutoMapper.Mapper.CreateMap<IInvoiceStatus, InvoiceStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IInvoiceLineItem, InvoiceLineItemDisplay>();
            AutoMapper.Mapper.CreateMap<IInvoice, InvoiceDisplay>();

            // Order
            AutoMapper.Mapper.CreateMap<IOrderStatus, OrderStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IOrderLineItem, OrderLineItemDisplay>();
            AutoMapper.Mapper.CreateMap<IOrder, OrderDisplay>();

            // payment
            AutoMapper.Mapper.CreateMap<IAppliedPayment, AppliedPaymentDisplay>();
            AutoMapper.Mapper.CreateMap<IPayment, PaymentDisplay>();

            AutoMapper.Mapper.CreateMap<IPaymentMethod, PaymentMethodDisplay>();

            AutoMapper.Mapper.CreateMap<IPaymentGatewayMethod, PaymentMethodDisplay>()
                .ForMember(dest => dest.Key,
                    opt => opt.MapFrom(src => src.PaymentMethod.Key)
                )
                .ForMember(dest => dest.ProviderKey,
                    opt => opt.MapFrom(src => src.PaymentMethod.ProviderKey)
                )
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.PaymentMethod.Name)
                )
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.PaymentMethod.Description)
                )
                .ForMember(dest => dest.PaymentCode,
                    opt => opt.MapFrom(src => src.PaymentMethod.PaymentCode)
                )
                .ForMember(dest => dest.DialogEditorView,
                    opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>()
                        .ConstructedBy(() => new GatewayMethodDialogEditorViewResolver())
                );

            // products
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            // Gateway Provider       
            AutoMapper.Mapper.CreateMap<IGatewayProvider, GatewayProviderDisplay>()                
                .ForMember(dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver())                    
                )
                .ForMember(dest => dest.DialogEditorView,
                    opt => opt.ResolveUsing<GatewayProviderDialogEditorViewResolver>().ConstructedBy(() => new GatewayProviderDialogEditorViewResolver())
                );
            

            AutoMapper.Mapper.CreateMap<IGatewayResource, GatewayResourceDisplay>();

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

            // shipment
            AutoMapper.Mapper.CreateMap<IShipment, ShipmentDisplay>();

            // taxation
            AutoMapper.Mapper.CreateMap<ITaxMethod, TaxMethodDisplay>();

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
            
            // warehouse
            AutoMapper.Mapper.CreateMap<IWarehouse, WarehouseDisplay>();
            AutoMapper.Mapper.CreateMap<IWarehouseCatalog, WarehouseCatalogDisplay>();            
        }
    }

}