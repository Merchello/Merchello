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
            CacheMap<ProductActual, ProductVariantDto>(src => src.Key, dto => dto.Key);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Name, dto => dto.Name);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Price, dto => dto.Price);
            CacheMap<ProductActual, ProductVariantDto>(src => src.CostOfGoods, dto => dto.CostOfGoods);
            CacheMap<ProductActual, ProductVariantDto>(src => src.SalePrice, dto => dto.SalePrice);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Weight, dto => dto.Weight);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Length, dto => dto.Length);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Height, dto => dto.Height);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Width, dto => dto.Width);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Barcode, dto => dto.Barcode);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Available, dto => dto.Available);
            CacheMap<ProductActual, ProductVariantDto>(src => src.TrackInventory, dto => dto.TrackInventory);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Taxable, dto => dto.Taxable);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Shippable, dto => dto.Shippable);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Download, dto => dto.Download);
            CacheMap<ProductActual, ProductVariantDto>(src => src.DownloadUrl, dto => dto.DownloadUrl);
            CacheMap<ProductActual, ProductVariantDto>(src => src.Template, dto => dto.Template);
            CacheMap<ProductActual, ProductVariantDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<ProductActual, ProductVariantDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
