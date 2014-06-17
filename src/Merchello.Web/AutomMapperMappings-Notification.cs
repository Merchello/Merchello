namespace Merchello.Web
{
    using Core.Gateways.Notification;
    using Core.Models;
    using Models.ContentEditing;
    using Models.MapperResolvers;

    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Creates notification mappings.
        /// </summary>
        private static void CreateNotificationMappings()
        {
            AutoMapper.Mapper.CreateMap<INotificationMessage, NotificationMessageDisplay>();

            // Notification Methods
            AutoMapper.Mapper.CreateMap<INotificationMethod, NotificationMethodDisplay>();

            AutoMapper.Mapper.CreateMap<INotificationGatewayMethod, NotificationMethodDisplay>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.NotificationMethod.Key))
                .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.NotificationMethod.ProviderKey))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NotificationMethod.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.NotificationMethod.Description))
                .ForMember(dest => dest.ServiceCode, opt => opt.MapFrom(src => src.NotificationMethod.ServiceCode))
                .ForMember(dest => dest.DialogEditorView, opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>().ConstructedBy(() => new GatewayMethodDialogEditorViewResolver()));
        }
    }
}