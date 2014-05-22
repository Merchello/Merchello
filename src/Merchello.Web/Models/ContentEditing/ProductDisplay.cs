using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductDisplay : ProductDisplayBase
    {
        public ProductDisplay()
        { }

        internal ProductDisplay(ProductVariantDisplay masterVariant)
        {
            Key = masterVariant.ProductKey;
            Name = masterVariant.Name;
            Sku = masterVariant.Sku;
            Price = masterVariant.Price;
            CostOfGoods = masterVariant.CostOfGoods;
            SalePrice = masterVariant.SalePrice;
            OnSale = masterVariant.OnSale;
            Manufacturer = masterVariant.Manufacturer;
            ManufacturerModelNumber = masterVariant.ManufacturerModelNumber;
            Weight = masterVariant.Weight;
            Length = masterVariant.Length;
            Height = masterVariant.Height;
            Barcode = masterVariant.Barcode;
            Available = masterVariant.Available;
            TrackInventory = masterVariant.TrackInventory;
            OutOfStockPurchase = masterVariant.OutOfStockPurchase;
            Taxable = masterVariant.Taxable;
            Shippable = masterVariant.Shippable;
            Download = masterVariant.Download;
            DownloadMediaId = masterVariant.DownloadMediaId;

            CatalogInventories = masterVariant.CatalogInventories;
        }

        public Guid Key { get; set; }        

        public IEnumerable<ProductOptionDisplay> ProductOptions { get; set; }
        public IEnumerable<ProductVariantDisplay> ProductVariants { get; set; }
      
    }
}
