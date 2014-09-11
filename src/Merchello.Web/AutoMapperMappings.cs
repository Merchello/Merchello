namespace Merchello.Web
{
    using System.Web.UI;

    using Core.Gateways;
    using Core.Models;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.SaleHistory;

    using Models.ContentEditing;
    using Models.MapperResolvers;

    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Responsible for creating the AutoMapper Mappings
        /// </summary>
        public static void CreateMappings()
        {
            // Address
            AutoMapper.Mapper.CreateMap<IAddress, AddressDisplay>();           
            AutoMapper.Mapper.CreateMap<AddressDisplay, Address>();

            // AuditLog
            AutoMapper.Mapper.CreateMap<IAuditLog, AuditLogDisplay>()
                .ForMember(
                    dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(
                    dest => dest.RecordDate,
                    opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(
                    dest => dest.EntityType,
                    opt => opt.ResolveUsing<EntityTypeResolver>().ConstructedBy(() => new EntityTypeResolver()));

            // Country and provinces
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();

            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            // Customer
            AutoMapper.Mapper.CreateMap<ICustomer, CustomerDisplay>()
                .ForMember(
                    dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()));

            AutoMapper.Mapper.CreateMap<ICustomerAddress, CustomerAddressDisplay>();


            // Gateway Provider       
            AutoMapper.Mapper.CreateMap<IGatewayProviderSettings, GatewayProviderDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.DialogEditorView, opt => opt.ResolveUsing<GatewayProviderDialogEditorViewResolver>().ConstructedBy(() => new GatewayProviderDialogEditorViewResolver()));

            AutoMapper.Mapper.CreateMap<IGatewayResource, GatewayResourceDisplay>();
            
            // Invoice
            AutoMapper.Mapper.CreateMap<IInvoiceStatus, InvoiceStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IInvoiceLineItem, InvoiceLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()));

            AutoMapper.Mapper.CreateMap<IInvoice, InvoiceDisplay>();

            // Order
            AutoMapper.Mapper.CreateMap<IOrderStatus, OrderStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IOrderLineItem, OrderLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()));

            AutoMapper.Mapper.CreateMap<IOrder, OrderDisplay>();
            
            // setup the other mappings
            CreateShippingMappings();

            CreateTaxationMappings();

            CreatePaymentMappings();

            CreateWarehouseAndProductMappings();

            CreateNotificationMappings();

            // ProductContentEditing.BindMappings();
        }
    }
}