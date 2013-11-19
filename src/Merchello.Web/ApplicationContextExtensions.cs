using Merchello.Core;
using Merchello.Core.Services;
using ServiceContext = Umbraco.Core.Services.ServiceContext;

namespace Merchello.Web
{
    /// <summary>
    /// Extension methods to expose Merchello services on the ApplicationContext class
    /// </summary>
    public static class ApplicationContextExtensions
    {

        #region  ServiceContext        

        /// <summary>
        /// Exposes Merchello's ProductService on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IProductService ProductService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.ProductService;
        }

        /// <summary>
        /// Exposes Merchello's ProductVariantService on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IProductVariantService ProductVariantService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.ProductVariantService;
        }

        /// <summary>
        /// Exposes Merchello's CustomerService on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ICustomerService CustomerService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.CustomerService;
        }

        /// <summary>
        /// Exposes Merchello's WarehouseService on the ApplicationContext.ServiceContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IWarehouseService WarehouseService(this ServiceContext context)
        {
            return MerchelloContext.Current.Services.WarehouseService;
        }

        #endregion
    }
}