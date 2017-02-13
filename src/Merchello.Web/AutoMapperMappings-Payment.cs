using Merchello.Web.Models.Payments;

namespace Merchello.Web
{
    using System.Linq;
    using Core.Gateways.Payment;
    using Core.Models;

    using Merchello.Core;

    using Models.ContentEditing;
    using Models.MapperResolvers;

    /// <summary>
    /// Binds Merchello AutoMapper mappings during the Umbraco startup.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Creates payment mappings.
        /// </summary>
        private static void CreatePaymentMappings()
        {
            // payment
            AutoMapper.Mapper.CreateMap<IAppliedPayment, AppliedPaymentDisplay>();

            AutoMapper.Mapper.CreateMap<IPayment, PaymentDisplay>()
                .ForMember(dest => dest.ExtendedData, opt => opt.ResolveUsing<ExtendedDataResolver>().ConstructedBy(() => new ExtendedDataResolver()))
                .ForMember(dest => dest.AppliedPayments, opt => opt.MapFrom(src => src.AppliedPayments().Select(x => x.ToAppliedPaymentDisplay())));

            AutoMapper.Mapper.CreateMap<IPaymentMethod, PaymentMethodDisplay>();

            AutoMapper.Mapper.CreateMap<IPaymentGatewayMethod, PaymentMethodDisplay>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.PaymentMethod.Key))
                .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.PaymentMethod.ProviderKey))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PaymentMethod.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.PaymentMethod.Description))
                .ForMember(dest => dest.PaymentCode, opt => opt.MapFrom(src => src.PaymentMethod.PaymentCode))
                .ForMember(
                    dest => dest.DialogEditorView,
                    opt =>
                    opt.ResolveUsing<GatewayMethodDialogEditorViewResolver>()
                        .ConstructedBy(() => new GatewayMethodDialogEditorViewResolver()))
                .ForMember(
                    dest => dest.AuthorizePaymentEditorView,
                    opt =>
                    opt.ResolveUsing<AuthorizePaymentDialogEditorViewResolver>()
                        .ConstructedBy(() => new AuthorizePaymentDialogEditorViewResolver()))
                .ForMember(
                    dest => dest.AuthorizeCapturePaymentEditorView,
                    opt =>
                    opt.ResolveUsing<AuthorizeCapturePaymentDialogEditorViewResolver>()
                        .ConstructedBy(() => new AuthorizeCapturePaymentDialogEditorViewResolver()))
                .ForMember(
                    dest => dest.RefundPaymentEditorView,
                    opt =>
                    opt.ResolveUsing<RefundPaymentDialogEditorViewResolver>()
                        .ConstructedBy(() => new RefundPaymentDialogEditorViewResolver()))
                .ForMember(
                    dest => dest.VoidPaymentEditorView,
                    opt =>
                    opt.ResolveUsing<VoidPaymentDialogEditorViewResolver>()
                        .ConstructedBy(() => new VoidPaymentDialogEditorViewResolver()))
                .ForMember(
                    dest => dest.CapturePaymentEditorView,
                    opt =>
                    opt.ResolveUsing<CapturePaymentDialogEditorViewResolver>()
                        .ConstructedBy(() => new CapturePaymentDialogEditorViewResolver()))
                .ForMember(
                    dest => dest.IncludeInPaymentSelection,
                    opt =>
                    opt.ResolveUsing<IncludeInPaymentSelectionResolver>()
                        .ConstructedBy(() => new IncludeInPaymentSelectionResolver()))
                .ForMember(
                    dest => dest.RequiresCustomer,
                    opt =>
                    opt.ResolveUsing<RequiresCustomerResolver>()
                        .ConstructedBy(() => new RequiresCustomerResolver()));

        }
    }
}