using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public abstract class ProductDisplayBase
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal CostOfGoods { get; set; }
        public decimal SalePrice { get; set; }
        public bool OnSale { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerModelNumber { get; set; }
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

        public IEnumerable<WarehouseInventoryDisplay> WarehouseInventory { get; set; }
    }
}