namespace Merchello.Providers
{
    using global::Braintree;

    using Merchello.Providers.Payment.Braintree.Models;

    /// <summary>
    /// Creates the Braintree AutoMapper mappings.
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// Creates the Braintree mappings.
        /// </summary>
        public static void CreateBraintreeMappings()
        {
            AutoMapper.Mapper.CreateMap<MerchantDescriptor, DescriptorRequest>();
        }  
    }
}
