namespace Merchello.Providers
{
    /// <summary>
    /// Creates AutoMapper mappings - used in <see cref="UmbracoApplicationEvents"/>
    /// </summary>
    internal static partial class AutoMapperMappings
    {
        /// <summary>
        /// The create mappings.
        /// </summary>
        public static void CreateMappings()
        {
            // Actual mappings are done in nested files
            // AutoMapperMappings-AuthorizeNet.cs
            // AutoMapperMappings-Braintree.cs
            // AutoMapperMappings-PayPal.cs

            // Authorize.NET
            CreateAuthorizeNetMappings();

            // Braintree
            CreateBraintreeMappings();

            // PayPal
            CreatePayPalMappings();
        }
    }
}