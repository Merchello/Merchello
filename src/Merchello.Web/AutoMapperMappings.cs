namespace Merchello.Web
{
    using Core.Gateways;
    using Core.Models;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Strategies.Itemization;
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

            AutoMapper.Mapper.CreateMap<ICurrency, CurrencyDisplay>();

            // Country and provinces
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();

            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            // Customer
            AutoMapper.Mapper.CreateMap<ICustomer, CustomerDisplay>()
                .ForMember(
                    dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(
                    dest => dest.Invoices,
                    opt => opt.ResolveUsing<CustomerInvoicesResolver>().ConstructedBy(() => new CustomerInvoicesResolver()));

            // Customer Basket
            AutoMapper.Mapper.CreateMap<IItemCacheLineItem, ItemCacheLineItemDisplay>()
               .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
               .ForMember(dest => dest.LineItemTypeField, opt => opt.ResolveUsing<LineItemTypeFieldResolver>().ConstructedBy(() => new LineItemTypeFieldResolver()));

            AutoMapper.Mapper.CreateMap<IItemCache, CustomerItemCacheDisplay>()
                .ForMember(
                dest => dest.Customer,
                opt => opt.ResolveUsing<ItemCacheCustomerResolver>().ConstructedBy(() => new ItemCacheCustomerResolver()));

            AutoMapper.Mapper.CreateMap<ICustomerAddress, CustomerAddressDisplay>();

            // Gateway Provider    
            AutoMapper.Mapper.CreateMap<IGatewayProviderSettings, GatewayProviderDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.DialogEditorView, opt => opt.ResolveUsing<GatewayProviderDialogEditorViewResolver>().ConstructedBy(() => new GatewayProviderDialogEditorViewResolver()));

            AutoMapper.Mapper.CreateMap<IGatewayResource, GatewayResourceDisplay>();
           
            // Note
            AutoMapper.Mapper.CreateMap<INote, NoteDisplay>()
                .ForMember(
                    dest => dest.EntityType,
                    opt => opt.ResolveUsing<NoteEntityTypeResolver>().ConstructedBy(() => new NoteEntityTypeResolver()))
                .ForMember(
                    dest => dest.NoteTypeField,
                    opt => opt.ResolveUsing<NoteTypeFieldResolver>().ConstructedBy(() => new NoteTypeFieldResolver()))
                .ForMember(
                    dest => dest.RecordDate,
                    opt => opt.MapFrom(src => src.CreateDate));

            // Invoice
            AutoMapper.Mapper.CreateMap<IInvoiceStatus, InvoiceStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IInvoiceLineItem, InvoiceLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.LineItemTypeField, opt => opt.ResolveUsing<LineItemTypeFieldResolver>().ConstructedBy(() => new LineItemTypeFieldResolver()));

            AutoMapper.Mapper.CreateMap<IInvoice, InvoiceDisplay>();

            AutoMapper.Mapper.CreateMap<InvoiceItemItemization, InvoiceItemItemizationDisplay>();

            // Order
            AutoMapper.Mapper.CreateMap<IOrderStatus, OrderStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IOrderLineItem, OrderLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.LineItemTypeField, opt => opt.ResolveUsing<LineItemTypeFieldResolver>().ConstructedBy(() => new LineItemTypeFieldResolver()));

            AutoMapper.Mapper.CreateMap<IOrder, OrderDisplay>();
                      
            // setup the other mappings
            CreateDetachedContentMappings();

            CreateEntityCollectionMappings();

            CreateMarketingMappings();

            CreateShippingMappings();

            CreateTaxationMappings();

            CreatePaymentMappings();

            CreateWarehouseAndProductMappings();

            CreateNotificationMappings();

            // ProductContentEditing.BindMappings();
        }
    }
}