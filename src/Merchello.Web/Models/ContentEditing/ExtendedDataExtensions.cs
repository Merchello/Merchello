namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Globalization;

    using Merchello.Core;
    using Merchello.Core.Models;

    public static class ExtendedDataExtensions
    {
        /// <summary>
        /// The add product variant values.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
        public static void AddProductVariantValues(this ExtendedDataCollection extendedData, ProductVariantDisplay productVariant)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.ProductKey, productVariant.ProductKey.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.ProductVariantKey, productVariant.Key.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.CostOfGoods, productVariant.CostOfGoods.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Weight, productVariant.Weight.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Width, productVariant.Width.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Height, productVariant.Height.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Length, productVariant.Length.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Barcode, productVariant.Barcode);
            extendedData.SetValue(Constants.ExtendedDataKeys.Price, productVariant.Price.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.OnSale, productVariant.OnSale.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Manufacturer, productVariant.Manufacturer);
            extendedData.SetValue(Constants.ExtendedDataKeys.ManufacturerModelNumber, productVariant.ManufacturerModelNumber);
            extendedData.SetValue(Constants.ExtendedDataKeys.SalePrice, productVariant.SalePrice.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.TrackInventory, productVariant.TrackInventory.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.OutOfStockPurchase, productVariant.OutOfStockPurchase.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Taxable, productVariant.Taxable.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Shippable, productVariant.Shippable.ToString());           
            extendedData.SetValue(Constants.ExtendedDataKeys.Download, productVariant.Download.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.DownloadMediaId, productVariant.DownloadMediaId.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.VersionKey, productVariant.VersionKey.ToString());
        }

        /// <summary>
        /// Adds product values
        /// </summary>
        /// <param name="extendedData">
        /// The extended data
        /// </param>
        /// <param name="product">
        /// The product variant
        /// </param>
        public static void AddProductValues(this ExtendedDataCollection extendedData, ProductDisplay product)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.ProductKey, product.Key.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.ProductVariantKey, product.ProductVariantKey.ToString());
            AddBaseProductValues(extendedData, product);
        }

        /// <summary>
        /// Adds product base values
        /// </summary>
        /// <param name="extendedData">
        /// The extended data
        /// </param>
        /// <param name="productBase">
        /// The base product
        /// </param>
        private static void AddBaseProductValues(ExtendedDataCollection extendedData, ProductDisplayBase productBase)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.CostOfGoods, productBase.CostOfGoods.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Weight, productBase.Weight.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Width, productBase.Width.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Height, productBase.Height.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Length, productBase.Length.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.Barcode, productBase.Barcode);
            extendedData.SetValue(Constants.ExtendedDataKeys.Price, productBase.Price.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.OnSale, productBase.OnSale.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Manufacturer, productBase.Manufacturer);
            extendedData.SetValue(Constants.ExtendedDataKeys.ManufacturerModelNumber, productBase.ManufacturerModelNumber);
            extendedData.SetValue(Constants.ExtendedDataKeys.SalePrice, productBase.SalePrice.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.TrackInventory, productBase.TrackInventory.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.OutOfStockPurchase, productBase.OutOfStockPurchase.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Taxable, productBase.Taxable.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Shippable, productBase.Shippable.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Download, productBase.Download.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.DownloadMediaId, productBase.DownloadMediaId.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.VersionKey, productBase.VersionKey.ToString());
        }

        #region INote

        /// <summary>
        /// Adds an <see cref="NoteDisplay"/> to extended data.  This is intended for a note against the sale (delivery instructions, etc)
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="note">
        /// The note.
        /// </param>
        public static void AddNote(this ExtendedDataCollection extendedData, NoteDisplay note)
        {
            var noteXml = SerializationHelper.SerializeToXml(note as NoteDisplay);

            extendedData.SetValue(Constants.ExtendedDataKeys.Note, noteXml);
        }

        /// <summary>
        /// The get note.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="NoteDisplay"/>.
        /// </returns>
        [Obsolete("Use ICheckoutManager.Extended.GetNotes - returns IEnumerable<string>")]
        public static NoteDisplay GetNote(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.Note)) return null;

            var attempt = SerializationHelper.DeserializeXml<NoteDisplay>(extendedData.GetValue(Constants.ExtendedDataKeys.Note));

            return attempt.Success ? attempt.Result : null;
        }


        #endregion

    }
}