namespace Merchello.Web
{
    using Core.Gateways;
    using Core.Models;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.MapperResolvers.Offers;
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
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(
                    dest => dest.Invoices,
                    opt => opt.ResolveUsing<CustomerInvoicesResolver>().ConstructedBy(() => new CustomerInvoicesResolver()));

            AutoMapper.Mapper.CreateMap<ICustomerAddress, CustomerAddressDisplay>();


            // Gateway Provider    
            AutoMapper.Mapper.CreateMap<IGatewayProviderSettings, GatewayProviderDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.DialogEditorView, opt => opt.ResolveUsing<GatewayProviderDialogEditorViewResolver>().ConstructedBy(() => new GatewayProviderDialogEditorViewResolver()));

            AutoMapper.Mapper.CreateMap<IGatewayResource, GatewayResourceDisplay>();
           

            // Invoice
            AutoMapper.Mapper.CreateMap<IInvoiceStatus, InvoiceStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IInvoiceLineItem, InvoiceLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.LineItemTypeField, opt => opt.ResolveUsing<LineItemTypeFieldResolver>().ConstructedBy(() => new LineItemTypeFieldResolver()));

            AutoMapper.Mapper.CreateMap<IInvoice, InvoiceDisplay>();

            // Order
            AutoMapper.Mapper.CreateMap<IOrderStatus, OrderStatusDisplay>();
            AutoMapper.Mapper.CreateMap<IOrderLineItem, OrderLineItemDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.LineItemTypeField, opt => opt.ResolveUsing<LineItemTypeFieldResolver>().ConstructedBy(() => new LineItemTypeFieldResolver()));

            AutoMapper.Mapper.CreateMap<IOrder, OrderDisplay>();
            
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
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("editorVeiw")))
                .ForMember(
                    dest => dest.RestictToType,
                    opt =>
                    opt.ResolveUsing<OfferComponentAttributeValueResolver>()
                        .ConstructedBy(() => new OfferComponentAttributeValueResolver("restrictToType")));

            AutoMapper.Mapper.CreateMap<IOfferProvider, OfferProviderDisplay>()
                .ForMember(
                    dest => dest.BackOfficeTree,
                    opt =>
                    opt.ResolveUsing<OfferProviderBackOfficeAttributeValueResolver>()
                        .ConstructedBy(() => new OfferProviderBackOfficeAttributeValueResolver()));

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