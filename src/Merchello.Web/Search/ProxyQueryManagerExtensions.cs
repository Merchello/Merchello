namespace Merchello.Web.Search
{
    using Merchello.Core;

    /// <summary>
    /// Extension methods for <see cref="IProxyQueryManager"/>.
    /// </summary>
    internal static class ProxyQueryManagerExtensions
    {
        /// <summary>
        /// Gets an instance of the <see cref="IProductCollectionQuery"/>.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The <see cref="IProductCollectionQuery"/>.
        /// </returns>
        public static IProductCollectionQuery CollectionQuery(this IProxyQueryManager manager)
        {
            return manager.Instance<ProductCollectionQuery>(new object[] { MerchelloContext.Current });
        }

        /// <summary>
        /// Gets an instance of the <see cref="IProductFilterGroupQuery"/>.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The <see cref="IProductFilterGroupQuery"/>.
        /// </returns>
        public static IProductFilterGroupQuery FilterGroupQuery(this IProxyQueryManager manager)
        {
            return manager.Instance<ProductFilterGroupQuery>(new object[] { MerchelloContext.Current });
        }
    }
}