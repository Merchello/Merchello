namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The digital media mapper.
    /// </summary>
    internal sealed class DigitalMediaMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaMapper"/> class.
        /// </summary>
        public DigitalMediaMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// Maps fields between DigitalMedia and DigitalMediaDto classes.  Used to allow strongly typed queries in repositories
        /// and services
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<DigitalMedia, DigitalMediaDto>(src => src.Key, dto => dto.Key);
            CacheMap<DigitalMedia, DigitalMediaDto>(src => src.ProductVariantKey, dto => dto.ProductVariantKey);
            CacheMap<DigitalMedia, DigitalMediaDto>(src => src.FirstAccessed, dto => dto.FirstAccessed);
            CacheMap<DigitalMedia, DigitalMediaDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<DigitalMedia, DigitalMediaDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}