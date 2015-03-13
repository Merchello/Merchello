﻿using System.Collections.Generic;

namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// A class responsible for building ProductVariant entities and DTO objects.
    /// </summary>
    internal class ProductVariantFactory : IEntityFactory<IProductVariant, ProductVariantDto>
    {
        /// <summary>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </summary>
        private readonly ProductAttributeCollection _productAttributeCollection;

        /// <summary>
        /// The <see cref="CatalogInventoryCollection"/>.
        /// </summary>
        private readonly CatalogInventoryCollection _catalogInventories;
        
        public ProductVariantFactory()
        {
            _productAttributeCollection = new ProductAttributeCollection();
            _catalogInventories = new CatalogInventoryCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantFactory"/> class.
        /// </summary>
        /// <param name="productAttributes">
        /// The product attributes.
        /// </param>
        /// <param name="catalogInventories">
        /// The catalog inventories.
        /// </param>
        public ProductVariantFactory(ProductAttributeCollection productAttributes,
            CatalogInventoryCollection catalogInventories)
        {
            _productAttributeCollection = productAttributes;
            _catalogInventories = catalogInventories;
        }
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
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

        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        public IProductVariant BuildEntity(ProductVariantDto dto, IEnumerable<ProductAttributeDto> attributeDtos, IEnumerable<CatalogInventoryDto> inventoryDtos)
        {
            var entity = (ProductVariant)BuildEntity(dto);

            var attrFactory = new ProductAttributeFactory();
            foreach (var attributeDto in attributeDtos)
            {
                entity.ProductAttributes.Add(attrFactory.BuildEntity(attributeDto));
            }
            var invFactory = new CatalogInventoryFactory();
            foreach (var invDto in inventoryDtos)
            {
                entity.CatalogInventoryCollection.Add(invFactory.BuildEntity(invDto));
            }

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