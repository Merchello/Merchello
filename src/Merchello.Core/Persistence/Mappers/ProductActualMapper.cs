using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="ProductActual"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class ProductActualMapper : MerchelloBaseMapper
    {
        public ProductActualMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<ProductActual, ProductActualDto>(src => src.Key, dto => dto.Key);
            CacheMap<ProductActual, ProductActualDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<ProductActual, ProductActualDto>(src => src.Name, dto => dto.Name);
            CacheMap<ProductActual, ProductActualDto>(src => src.Price, dto => dto.Price);
            CacheMap<ProductActual, ProductActualDto>(src => src.CostOfGoods, dto => dto.CostOfGoods);
            CacheMap<ProductActual, ProductActualDto>(src => src.SalePrice, dto => dto.SalePrice);
            CacheMap<ProductActual, ProductActualDto>(src => src.Weight, dto => dto.Weight);
            CacheMap<ProductActual, ProductActualDto>(src => src.Length, dto => dto.Length);
            CacheMap<ProductActual, ProductActualDto>(src => src.Height, dto => dto.Height);
            CacheMap<ProductActual, ProductActualDto>(src => src.Width, dto => dto.Width);
            CacheMap<ProductActual, ProductActualDto>(src => src.Barcode, dto => dto.Barcode);
            CacheMap<ProductActual, ProductActualDto>(src => src.Available, dto => dto.Available);
            CacheMap<ProductActual, ProductActualDto>(src => src.TrackInventory, dto => dto.TrackInventory);
            CacheMap<ProductActual, ProductActualDto>(src => src.Taxable, dto => dto.Taxable);
            CacheMap<ProductActual, ProductActualDto>(src => src.Shippable, dto => dto.Shippable);
            CacheMap<ProductActual, ProductActualDto>(src => src.Download, dto => dto.Download);
            CacheMap<ProductActual, ProductActualDto>(src => src.DownloadUrl, dto => dto.DownloadUrl);
            CacheMap<ProductActual, ProductActualDto>(src => src.Template, dto => dto.Template);
            CacheMap<ProductActual, ProductActualDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<ProductActual, ProductActualDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
