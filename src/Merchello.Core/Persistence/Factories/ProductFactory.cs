using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal partial class ProductFactory : IEntityFactory<IProduct, ProductDto>
    {
        public IProduct BuildEntity(ProductDto dto)
        {
            var product = new Product()
            {
                Key = dto.Key,
                Sku = dto.Sku,
                Name = dto.Name,
                Price = dto.Price,
                CostOfGoods = dto.CostOfGoods,
                SalePrice = dto.SalePrice,
                Weight = dto.Weight,
                Length = dto.Length,
                Width = dto.Width,
                Height = dto.Height,                
                Taxable = dto.Taxable,
                Shippable = dto.Shippable,
                Download = dto.Download,
                DownloadUrl = dto.DownloadUrl,
                Template = dto.Template,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            product.ResetDirtyProperties();

            return product;
        }

        public ProductDto BuildDto(IProduct entity)
        {
            var dto = new ProductDto()
            {
                Key = entity.Key,
                Sku = entity.Sku,
                Name = entity.Name,
                Price = entity.Price,
                CostOfGoods = entity.CostOfGoods,
                SalePrice = entity.SalePrice,
                Weight = entity.Weight,
                Length = entity.Length,
                Width = entity.Width,
                Height = entity.Height,                
                Taxable = entity.Taxable,
                Shippable = entity.Shippable,
                Download = entity.Download,
                DownloadUrl = entity.DownloadUrl,
                Template = entity.Template,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
