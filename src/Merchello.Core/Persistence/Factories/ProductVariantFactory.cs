using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ProductVariantFactory : IEntityFactory<IProductVariant, ProductVariantDto>
    {
        private readonly ProductAttributeCollection _productAttributeCollection;
        private readonly CatalogInventoryCollection _catalogInventories;

        public ProductVariantFactory(ProductAttributeCollection productAttributes,
            CatalogInventoryCollection catalogInventories)
        {
            _productAttributeCollection = productAttributes;
            _catalogInventories = catalogInventories;
        }

        public IProductVariant BuildEntity(ProductVariantDto dto)
        {
            var entity = new ProductVariant(dto.Name, dto.Sku, dto.Price)
            {
                Key = dto.Key,
                ProductKey = dto.ProductKey,
                CostOfGoods = dto.CostOfGoods,
                SalePrice = dto.SalePrice,
                OnSale = dto.OnSale,
                Manufacturer = dto.Manufacturer,
                ManufacturerModelNumber = dto.ManufacturerModelNumber,
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
                Master = dto.Master,
                ExamineId = dto.ProductVariantIndexDto.Id,   
                CatalogInventoryCollection = _catalogInventories,
                ProductAttributes = _productAttributeCollection,
                VersionKey = dto.VersionKey,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            entity.ResetDirtyProperties();
            return entity;
        }

        public ProductVariantDto BuildDto(IProductVariant entity)
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
                Manufacturer = entity.Manufacturer,
                ManufacturerModelNumber = entity.ManufacturerModelNumber,
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
                Master = ((ProductVariant)entity).Master,
                ProductVariantIndexDto = new ProductVariantIndexDto()
                    {
                      Id = ((ProductVariant)entity).ExamineId,
                      ProductVariantKey = entity.Key,
                      UpdateDate = entity.UpdateDate,
                      CreateDate = entity.CreateDate
                    },
                VersionKey = entity.VersionKey,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };
        }
    }
}