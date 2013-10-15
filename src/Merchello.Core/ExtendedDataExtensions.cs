using Merchello.Core.Models;

namespace Merchello.Core
{
    /// <summary>
    /// Extension methods for <see cref="ExtendedDataCollection"/>
    /// </summary>
    public static class ExtendedDataCollectionExtensions
    {

        public static void AddProductVariantValues(this ExtendedDataCollection extendedData, IProductVariant productVariant)
        {
            extendedData.SetValue("MerchProductKey", productVariant.ProductKey.ToString());
            extendedData.SetValue("MerchProductVariantKey", productVariant.Key.ToString());
            extendedData.SetValue("MerchCostOfGoods", productVariant.CostOfGoods.ToString());
            extendedData.SetValue("MerchWeight", productVariant.Weight.ToString());
            extendedData.SetValue("MerchWidth", productVariant.Width.ToString());
            extendedData.SetValue("MerchHeight", productVariant.Height.ToString());
            extendedData.SetValue("MerchBarcode", productVariant.Barcode);
            extendedData.SetValue("MerchTrackInventory", productVariant.TrackInventory.ToString());
            extendedData.SetValue("MerchOutOfStockPurchase", productVariant.OutOfStockPurchase.ToString());
            extendedData.SetValue("MerchTaxable", productVariant.Taxable.ToString());
            extendedData.SetValue("MerchShippable", productVariant.Shippable.ToString());
            extendedData.SetValue("MerchDownload", productVariant.Download.ToString());
            extendedData.SetValue("MerchDownloadMediaId", productVariant.DownloadMediaId.ToString());
        }
    }
}