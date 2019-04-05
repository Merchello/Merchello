namespace Merchello.Providers.Payment.Braintree
{
    using AutoMapper;
    using global::Braintree;
    using Merchello.Providers.Payment.Braintree.Models;

    public class AutomapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<MerchantDescriptor, DescriptorRequest>();
        }
    }
}
