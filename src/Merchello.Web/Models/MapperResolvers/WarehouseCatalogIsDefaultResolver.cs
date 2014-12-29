namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;
    using Core.Models;

    /// <summary>
    /// The warehouse catalog is default resolver.
    /// </summary>
    public class WarehouseCatalogIsDefaultResolver : ValueResolver<IWarehouseCatalog, bool>
    {
        /// <summary>
        /// Executes the resolution of the value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool ResolveCore(IWarehouseCatalog source)
        {
            return source.Key.Equals(Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey);
        }
    }
}