using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class ProductDisplay : ProductDisplayBase
    {
        public ProductDisplay()
        {
        }

        public Guid Key { get; set; }        

        public IEnumerable<ProductOptionDisplay> ProductOptions { get; set; }
        public IEnumerable<ProductVariantDisplay> ProductVariants { get; set; }

       

        ///// Move this to an extensions class on ProductDisplay
        //public IProduct ToProduct(IProduct destination)
        //{
        //    destination.Name = Name;
        //    destination.Sku = Sku;
        //    destination.Price = Price;
        //    destination.CostOfGoods = CostOfGoods;
        //    destination.SalePrice = SalePrice;
        //    destination.OnSale = OnSale;
        //    destination.Weight = Weight;
        //    destination.Length = Length;
        //    destination.Width = Width;
        //    destination.Height = Height;
        //    destination.Barcode = Barcode;
        //    destination.Available = Available;
        //    destination.TrackInventory = TrackInventory;
        //    destination.OutOfStockPurchase = OutOfStockPurchase;
        //    destination.Taxable = Taxable;
        //    destination.Shippable = Shippable;
        //    destination.Download = Download;
        //    destination.DownloadMediaId = DownloadMediaId;

        //    return destination;
        //}
    }
}
