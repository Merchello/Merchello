using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{    
    public static class ProductDisplayExtensions
    {

        public static IProduct ToProduct(this ProductDisplay productDisplay, string name, string sku, decimal price)
        {
            var destination = MerchelloContext.Current.Services.ProductService.CreateProduct(name, sku, price);
            return ToProduct(productDisplay, destination);
        }

        public static IProduct ToProduct(this ProductDisplay productDisplay, IProduct destination)
        {
            destination.Name = productDisplay.Name;
            destination.Sku = productDisplay.Sku;
            destination.Price = productDisplay.Price;
            destination.CostOfGoods = productDisplay.CostOfGoods;
            destination.SalePrice = productDisplay.SalePrice;
            destination.OnSale = productDisplay.OnSale;
            destination.Weight = productDisplay.Weight;
            destination.Length = productDisplay.Length;
            destination.Width = productDisplay.Width;
            destination.Height = productDisplay.Height;
            destination.Barcode = productDisplay.Barcode;
            destination.Available = productDisplay.Available;
            destination.TrackInventory = productDisplay.TrackInventory;
            destination.OutOfStockPurchase = productDisplay.OutOfStockPurchase;
            destination.Taxable = productDisplay.Taxable;
            destination.Shippable = productDisplay.Shippable;
            destination.Download = productDisplay.Download;
            destination.DownloadMediaId = productDisplay.DownloadMediaId;

            foreach (var option in productDisplay.ProductOptions)
            {
                IProductOption destinationProductOption;
                if (destination.ProductOptions.Contains(option.Name))
                {
                    destinationProductOption = destination.ProductOptions[option.Name];

                    destinationProductOption = option.ToProductOption(destinationProductOption);
                }
                else
                {
                    destinationProductOption = new ProductOption(option.Name, option.Required);

                    destinationProductOption = option.ToProductOption(destinationProductOption);
                }

                destination.ProductOptions.Add(destinationProductOption);
            }

            return destination;
        }
         
    }
}