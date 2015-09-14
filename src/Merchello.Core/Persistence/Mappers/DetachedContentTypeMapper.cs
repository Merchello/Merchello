namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The detached content type mapper.
    /// </summary>
    internal sealed class DetachedContentTypeMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeMapper"/> class.
        /// </summary>
        public DetachedContentTypeMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// The build map.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.Key, dto => dto.Key);
            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.Name, dto => dto.Name);
            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.Description, dto => dto.Description);
            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.EntityTfKey, dto => dto.EntityTfKey);
            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.ContentTypeKey, dto => dto.ContentTypeKey);
            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<DetachedContentType, DetachedContentTypeDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}