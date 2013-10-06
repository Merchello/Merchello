using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ProductFactory : IEntityFactory<IProduct, ProductDto>
    {
        public IProduct BuildEntity(ProductDto dto)
        {
            var variant = BuildProductVariant(dto.ProductVariantDto);
            var product = new Product(variant)
            {
                Key = dto.Key,
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
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate,
                ProductVariantDto = BuildProductVariantDto(((Product)entity).ProductVariantTemplate)
            };

            return dto;
        }


        private IProductVariant BuildProductVariant(ProductVariantDto dto)
        {
            var entity = new ProductVariant(dto.Name, dto.Sku, dto.Price)
            {
                Key = dto.Key,
                ProductKey = dto.ProductKey,
                CostOfGoods = dto.CostOfGoods,
                SalePrice = dto.SalePrice,
                OnSale = dto.OnSale,
                Weight = dto.Weight,
                Length = dto.Length,
                Height = dto.Height,
                Width = dto.Width,
                Barcode = dto.Barcode,
                Available = dto.Available,
                TrackInventory = dto.TrackInventory,
                OutOfStockPurchase = dto.OutOfStockPurchase,
                Taxable = dto.Taxable,
                Shippable = dto.Shippable,
                Download = dto.Download,
                DownloadMediaId = dto.DownloadMediaId,
                Template = dto.Template,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            entity.ResetDirtyProperties();
            return entity;
        }

        private ProductVariantDto BuildProductVariantDto(IProductVariant entity)
        {
            return new ProductVariantDto()
            {
                Key = entity.Key,
                ProductKey = entity.ProductKey,
                Name = entity.Name,
                Sku = entity.Sku,
                Price = entity.Price,
                CostOfGoods = entity.CostOfGoods,
                SalePrice = entity.SalePrice,
                OnSale = entity.OnSale,
                Weight = entity.Weight,
                Length = entity.Length,
                Height = entity.Height,
                Width = entity.Width,
                Barcode = entity.Barcode,
                Available = entity.Available,
                TrackInventory = entity.TrackInventory,
                OutOfStockPurchase = entity.OutOfStockPurchase,
                Taxable = entity.Taxable,
                Shippable = entity.Shippable,
                Download = entity.Download,
                DownloadMediaId = entity.DownloadMediaId,
                Template = ((ProductVariant)entity).Template,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}
