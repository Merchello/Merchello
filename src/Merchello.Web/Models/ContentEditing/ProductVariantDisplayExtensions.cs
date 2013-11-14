using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{    
    public static class ProductVariantDisplayExtensions
    {

        public static IProductVariant ToProduct(this ProductVariantDisplay productVariantDisplay, IProductVariant destination)
        {
            destination.Name = productVariantDisplay.Name;
            destination.Sku = productVariantDisplay.Sku;
            destination.Price = productVariantDisplay.Price;
            destination.CostOfGoods = productVariantDisplay.CostOfGoods;
            destination.SalePrice = productVariantDisplay.SalePrice;
            destination.OnSale = productVariantDisplay.OnSale;
            destination.Weight = productVariantDisplay.Weight;
            destination.Length = productVariantDisplay.Length;
            destination.Width = productVariantDisplay.Width;
            destination.Height = productVariantDisplay.Height;
            destination.Barcode = productVariantDisplay.Barcode;
            destination.Available = productVariantDisplay.Available;
            destination.TrackInventory = productVariantDisplay.TrackInventory;
            destination.OutOfStockPurchase = productVariantDisplay.OutOfStockPurchase;
            destination.Taxable = productVariantDisplay.Taxable;
            destination.Shippable = productVariantDisplay.Shippable;
            destination.Download = productVariantDisplay.Download;
            destination.DownloadMediaId = productVariantDisplay.DownloadMediaId;

            destination.ProductKey = productVariantDisplay.ProductKey;

            // TODO: Warehouse Inventory


            // JASON: Not sure we event need to do this...
            //foreach (var attribute in productVariantDisplay.Attributes)
            //{
            //    IProductAttribute destinationProductAttribute;


            //    if (destination.Attributes.Contains(attribute.Name))
            //    {
            //        destinationProductOption = destination.ProductOptions[attribute.Name];

            //        destinationProductOption = attribute.ToProductOption(destinationProductOption);
            //    }
            //    else
            //    {
            //        destinationProductOption = new ProductOption(attribute.Name, attribute.Required);

            //        destinationProductOption = attribute.ToProductOption(destinationProductOption);
            //    }

            //    destination.ProductOptions.Add(destinationProductOption);
            //}

            return destination;
        }
         
    }
}