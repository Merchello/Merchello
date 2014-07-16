namespace Merchello.Web
{
    using Merchello.Core;
    using Merchello.Core.Services;
    using ServiceContext = Umbraco.Core.Services.ServiceContext;

    //// TODO if we want these extensions, we need to have them match the publically available services defined in Merchello's service context

    /// <summary>
    /// Extension methods to expose Merchello services on the ApplicationContext class
    /// </summary>
    public static class ApplicationContextExtensions
    {
        #region  ServiceContext        

        /// <summary>
        /// Exposes the Merchello <see cref="IProductService"/> on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context">The Umbraco service context</param>
        /// <returns>The <see cref="IProductService"/></returns>
        public static IProductService ProductService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.ProductService;
        }

        /// <summary>
        /// Exposes the Merchello <see cref="IProductVariantService"/> on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context">The Umbraco service context</param>
        /// <returns>The <see cref="IProductVariantService"/></returns>
        public static IProductVariantService ProductVariantService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.ProductVariantService;
        }

        /// <summary>
        /// Exposes the Merchello <see cref="ICustomerService"/> on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context">The Umbraco service context</param>
        /// <returns>The <see cref="ICustomerService"/></returns>
        public static ICustomerService CustomerService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.CustomerService;
        }

        #endregion
    }
}