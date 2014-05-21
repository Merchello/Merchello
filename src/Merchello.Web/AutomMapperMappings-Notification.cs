using System.Runtime.InteropServices;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Models;
using Merchello.Web.Models.ContentEditing;
using Merchello.Web.Models.MapperResolvers;

namespace Merchello.Web
{
    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        private static void BindNotificationMappings()
        {

            AutoMapper.Mapper.CreateMap<INotificationMessage, NotificationMessageDisplay>();

            // Notification Methods
            AutoMapper.Mapper.CreateMap<INotificationMethod, NotificationMethodDisplay>();

            AutoMapper.Mapper.CreateMap<INotificationGatewayMethod, NotificationMethodDisplay>()
                .ForMember(dest => dest.Key,
                    opt => opt.MapFrom(src => src.NotificationMethod.Key)
                )
                .ForMember(dest => dest.ProviderKey,
                    opt => opt.MapFrom(src => src.NotificationMethod.ProviderKey)
                )
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.NotificationMethod.Name)
                )
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.NotificationMethod.Description)
                )
                .ForMember(dest => dest.ServiceCode,
                    opt => opt.MapFrom(src => src.NotificationMethod.ServiceCode)
                )
                .ForMember(dest => dest.DialogEditorView,
                    opt => opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>()
                        .ConstructedBy(() => new GatewayMethodDialogEditorViewResolver())
                );
        }
    }
}