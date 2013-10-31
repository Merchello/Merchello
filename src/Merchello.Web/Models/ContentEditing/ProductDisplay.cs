using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductDisplay
    {
        public ProductDisplay()
        {
        }

        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal CostOfGoods { get; set; }
        public decimal SalePrice { get; set; }
        public bool OnSale { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Barcode { get; set; }
        public bool Available { get; set; }
        public bool TrackInventory { get; set; }
        public bool OutOfStockPurchase { get; set; }
        public bool Taxable { get; set; }
        public bool Shippable { get; set; }
        public bool Download { get; set; }
        public int DownloadMediaId { get; set; }

        public IEnumerable<IProductOption> ProductOptions { get; set; }

        // Product Option -> Choices = ProductAttribute

        public IEnumerable<IProductVariant> ProductVariants { get; set; }

        //public IEnumerable<IWarehouseInventory> WarehouseInventory { get; set; } // not in lucene

        //public IEnumerable<IProductAttribute> Attributes { get; set; }

        /// Move this to an extensions class on ProductDisplay
        public IProduct ToProduct(IProduct destination)
        {
            destination.Name = Name;
            destination.Sku = Sku;
            destination.Price = Price;
            destination.CostOfGoods = CostOfGoods;
            destination.SalePrice = SalePrice;
            destination.OnSale = OnSale;
            destination.Weight = Weight;
            destination.Length = Length;
            destination.Width = Width;
            destination.Height = Height;
            destination.Barcode = Barcode;
            destination.Available = Available;
            destination.TrackInventory = TrackInventory;
            destination.OutOfStockPurchase = OutOfStockPurchase;
            destination.Taxable = Taxable;
            destination.Shippable = Shippable;
            destination.Download = Download;
            destination.DownloadMediaId = DownloadMediaId;

            return destination;
        }
    }
}
