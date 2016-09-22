namespace Merchello.Core.Persistence.Mappers
{
    using System.Collections.Concurrent;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Represents a <see cref="Merchello.Core.Models.AnonymousCustomer"/> to DTO mapper used to translate the property
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    [MapperFor(typeof(AnonymousCustomer))]
    [MapperFor(typeof(IAnonymousCustomer))]
    internal sealed class AnonymousCustomerMapper : BaseMapper
    {
        /// <summary>
        /// The mapper specific instance of the the property info cache.
        /// </summary>
        private static readonly ConcurrentDictionary<string, DtoMapModel> PropertyInfoCacheInstance = new ConcurrentDictionary<string, DtoMapModel>();

        /// <inheritdoc/>
        internal override ConcurrentDictionary<string, DtoMapModel> PropertyInfoCache => PropertyInfoCacheInstance;


        /// <inheritdoc/>
        protected override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.Key, dto => dto.Key);
            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.LastActivityDate, dto => dto.LastActivityDate);
            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<AnonymousCustomer, AnonymousCustomerDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }

    }
}
