namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The audit log mapper.
    /// </summary>
    internal sealed class AuditLogMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogMapper"/> class.
        /// </summary>
        public AuditLogMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// The build map.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<AuditLog, AuditLogDto>(src => src.Key, dto => dto.Key);
            CacheMap<AuditLog, AuditLogDto>(src => src.EntityKey, dto => dto.EntityKey);
            CacheMap<AuditLog, AuditLogDto>(src => src.EntityTfKey, dto => dto.EntityTfKey);
            CacheMap<AuditLog, AuditLogDto>(src => src.Message, dto => dto.Message);
            CacheMap<AuditLog, AuditLogDto>(src => src.Verbosity, dto => dto.Verbosity);
            CacheMap<AuditLog, AuditLogDto>(src => src.IsError, dto => dto.IsError);
            CacheMap<AuditLog, AuditLogDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<AuditLog, AuditLogDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}