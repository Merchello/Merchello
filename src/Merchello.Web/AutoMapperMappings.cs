using AutoMapper;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.Models.MapperResolvers;
using Umbraco.Core.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        public static void BindMappings()
        {
            // Address
            AutoMapper.Mapper.CreateMap<IAddress, AddressDisplay>();           
            AutoMapper.Mapper.CreateMap<AddressDisplay, Address>();

                       
            // Country and provinces
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();

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

            
            // Invoice
            AutoMapper.Mapper.CreateMap<IInvoiceStatus, InvoiceStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IInvoiceLineItem, InvoiceLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver())
                );

            AutoMapper.Mapper.CreateMap<IInvoice, InvoiceDisplay>();

            // Order
            AutoMapper.Mapper.CreateMap<IOrderStatus, OrderStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IOrderLineItem, OrderLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver())
                );

            AutoMapper.Mapper.CreateMap<IOrder, OrderDisplay>();
            
            //  setup the other mappings
            BindShippingMappings();

            BindTaxationMappings();

            BindPaymentMappings();

            BindWarehouseAndProductMappings();

            ProductContentEditing.BindMappings();
        }
    }

}