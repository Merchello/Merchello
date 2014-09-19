namespace Merchello.Plugin.Payments.Braintree
{
    using global::Braintree;
    using Models;

    /// <summary>
    /// Creates AutoMapper mappings - used in <see cref="UmbracoApplicationEvents"/>
    /// </summary>
    internal static class AutoMapperMappings
    {
        /// <summary>
        /// The create mappings.
        /// </summary>
        public static void CreateMappings()
        {
            AutoMapper.Mapper.CreateMap<BraintreeProviderSettings, BraintreeGateway>();
            AutoMapper.Mapper.CreateMap<MerchantDescriptor, DescriptorRequest>();
        }
    }
}