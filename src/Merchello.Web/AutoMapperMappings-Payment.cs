using System.Linq;
using Merchello.Core.Gateways.Payment;
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
        private static void BindPaymentMappings()
        {
            // payment
            AutoMapper.Mapper.CreateMap<IAppliedPayment, AppliedPaymentDisplay>();

            AutoMapper.Mapper.CreateMap<IPayment, PaymentDisplay>()
                .ForMember(dest => dest.ExtendedData,
                    opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver())
                )
                .ForMember(dest => dest.AppliedPayments, 
                    opt => opt.MapFrom(src => src.AppliedPayments().Select(x => x.ToAppliedPaymentDisplay()))
                );

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

        }
    }
}