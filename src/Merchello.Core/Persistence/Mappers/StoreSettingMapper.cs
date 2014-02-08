using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class StoreSettingMapper : MerchelloBaseMapper
    {
         public StoreSettingMapper()
         {
             BuildMap();
         }

         internal override void BuildMap()
         {
             if (!PropertyInfoCache.IsEmpty) return;

             CacheMap<StoreSetting, StoreSettingDto>(src => src.Key, dto => dto.Key);
             CacheMap<StoreSetting, StoreSettingDto>(src => src.Name, dto => dto.Name);
             CacheMap<StoreSetting, StoreSettingDto>(src => src.Value, dto => dto.Value);
             CacheMap<StoreSetting, StoreSettingDto>(src => src.TypeName, dto => dto.TypeName);
             CacheMap<StoreSetting, StoreSettingDto>(src => src.UpdateDate, dto => dto.UpdateDate);
             CacheMap<StoreSetting, StoreSettingDto>(src => src.CreateDate, dto => dto.CreateDate);
         }
    }
}