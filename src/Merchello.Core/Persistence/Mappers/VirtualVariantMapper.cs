namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The virtual variant mapper.
    /// </summary>
    internal sealed class VirtualVariantMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantMapper"/> class.
        /// </summary>
        public VirtualVariantMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// Maps fields between VirtualVariant and VirtualVariantsDto classes.  Used to allow strongly typed queries in repositories
        /// and services
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<VirtualVariant, VirtualVariantsDto>(src => src.Key, dto => dto.Key);
            CacheMap<VirtualVariant, VirtualVariantsDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<VirtualVariant, VirtualVariantsDto>(src => src.ProductKey, dto => dto.ProductKey);
            CacheMap<VirtualVariant, VirtualVariantsDto>(src => src.Choices, dto => dto.Choices);
        }
    }
}