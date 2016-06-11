namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;

    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Search;

    /// <summary>
    /// The item cache customer resolver.
    /// </summary>
    internal class ItemCacheCustomerResolver : ValueResolver<IItemCache, CustomerDisplay>
    {
        /// <summary>
        /// The <see cref="CachedCustomerQuery"/>
        /// </summary>
        private static readonly CachedCustomerQuery CustomerQuery = new CachedCustomerQuery();

        /// <summary>
        /// Resolves the <see cref="CustomerDisplay"/> for the item cache.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        protected override CustomerDisplay ResolveCore(IItemCache source)
        {
            return CustomerQuery.GetByKey(source.EntityKey);
        }
    }
}