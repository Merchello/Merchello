namespace Merchello.Core.Persistence.Mappers
{
    using System.Collections.Concurrent;

    /// <summary>
    /// The merchello base mapper.
    /// </summary>
    internal abstract class MerchelloBaseMapper : BaseMapper
    {
        /// <summary>
        /// The property info cache.
        /// </summary>
        protected readonly ConcurrentDictionary<string, DtoMapModel> PropertyInfoCacheInstance = new ConcurrentDictionary<string, DtoMapModel>();

        /// <summary>
        /// Gets the property info cache.
        /// </summary>
        internal override ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache
        {
            get { return PropertyInfoCacheInstance; }
        }

        /// <summary>
        /// Responsible for building the map between the class and the DTO.
        /// </summary>
        internal abstract void BuildMap();
    }
}
