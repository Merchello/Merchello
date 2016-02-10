namespace Merchello.Providers.Payment
{
    using global::Braintree;

    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Providers.Payment.Braintree.Models;

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
            AutoMapper.Mapper.CreateMap<MerchantDescriptor, DescriptorRequest>();
        }
    }
}