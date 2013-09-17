using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Product"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class ProductMapper : MerchelloBaseMapper
    {
        public ProductMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<Product, ProductDto>(src => src.Key, dto => dto.Key);
            CacheMap<Product, ProductDto>(src => src.Sku, dto => dto.Sku);
            CacheMap<Product, ProductDto>(src => src.Name, dto => dto.Name);
            CacheMap<Product, ProductDto>(src => src.Price, dto => dto.Price);
            CacheMap<Product, ProductDto>(src => src.CostOfGoods, dto => dto.CostOfGoods);
            CacheMap<Product, ProductDto>(src => src.SalePrice, dto => dto.SalePrice);
            CacheMap<Product, ProductDto>(src => src.Weight, dto => dto.Weight);
            CacheMap<Product, ProductDto>(src => src.Length, dto => dto.Length);
            CacheMap<Product, ProductDto>(src => src.Height, dto => dto.Height);
            CacheMap<Product, ProductDto>(src => src.Width, dto => dto.Width);
            CacheMap<Product, ProductDto>(src => src.Taxable, dto => dto.Taxable);
            CacheMap<Product, ProductDto>(src => src.Shippable, dto => dto.Shippable);
            CacheMap<Product, ProductDto>(src => src.Download, dto => dto.Download);
            CacheMap<Product, ProductDto>(src => src.DownloadUrl, dto => dto.DownloadUrl);
            CacheMap<Product, ProductDto>(src => src.Template, dto => dto.Template);
            CacheMap<Product, ProductDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Product, ProductDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
