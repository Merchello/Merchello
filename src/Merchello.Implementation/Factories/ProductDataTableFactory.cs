namespace Merchello.Implementation.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Implementation.Models;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// The product data table factory.
    /// </summary>
    /// <typeparam name="TTable">
    /// The type of of <see cref="IProductDataTable{TTableRow}"/>
    /// </typeparam>
    /// <typeparam name="TRow">
    /// The type of <see cref="IProductDataTableRow"/>
    /// </typeparam>
    public class ProductDataTableFactory<TTable, TRow>
        where TTable : class, IProductDataTable<TRow>, new()
        where TRow : class, IProductDataTableRow, new()
    {
        /// <summary>
        /// Creates a <see cref="IProductDataTable{TRow}"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="productContent">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TTable"/>.
        /// </returns>
        public TTable Create(IProductContent productContent)
        {
            var table = new TTable { ProductKey = productContent.Key };
            var rows = new List<TRow> { this.Create(productContent, false) };
            rows.AddRange(productContent.ProductVariants.Select(variant => this.Create(variant)));

            // Associate the table rows
            table.Rows = rows;
            
            return OnCreate(table, productContent);
        }

        /// <summary>
        /// Creates a <see cref="IProductDataTable{TRow}"/> from <see cref="ProductDisplay"/>.
        /// </summary>
        /// <param name="productDisplay">
        /// The <see cref="ProductDisplay"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TTable"/>.
        /// </returns>
        public TTable Create(ProductDisplay productDisplay)
        {
            var table = new TTable { ProductKey = productDisplay.Key };
       
            var rows = new List<TRow> { this.Create(productDisplay, false) };
            rows.AddRange(productDisplay.ProductVariants.Select(variant => this.Create(variant)));

            // Associate the table rows
            table.Rows = rows;

            return OnCreate(table, productDisplay);
        }

        /// <summary>
        /// Creates a <see cref="IProductDataTableRow"/> from <see cref="IProductContentBase"/>.
        /// </summary>
        /// <param name="baseContent">
        /// The <see cref="IProductContentBase"/>.
        /// </param>
        /// <param name="isVariant">
        /// A value indicating whether or not the row represents a variant.
        /// </param>
        /// <returns>
        /// The <see cref="TRow"/>.
        /// </returns>
        public TRow Create(IProductContentBase baseContent, bool isVariant = true)
        {
            Guid productKey;
            Guid productVariantKey;
            bool isAvaliable;
            Guid[] matchKeys;
            var type = baseContent.GetType();
            if (baseContent is IProductVariantContent)
            {
                var variant = (IProductVariantContent)baseContent;
                productKey = variant.ProductKey;
                productVariantKey = variant.Key;
                isAvaliable = variant.Available;
                matchKeys = variant.Attributes.Select(x => x.Key).ToArray();
            }
            else
            {
                var product = baseContent as IProductContent;
                if (product == null) throw new InvalidCastException("baseContent could not cast to IProductContent");
                productKey = product.Key;
                productVariantKey = product.ProductVariantKey;
                isAvaliable = product.Available;
                matchKeys = Enumerable.Empty<Guid>().ToArray();
            }

            var row = new TRow
                {
                    ProductKey = productKey,
                    ProductVariantKey = productVariantKey,
                    Sku = baseContent.Sku,
                    MatchKeys = matchKeys,
                    OnSale = baseContent.OnSale,
                    FormattedPrice = baseContent.Price.AsFormattedCurrency(),
                    Price = baseContent.Price,
                    SalePrice = baseContent.SalePrice,
                    FormattedSalePrice = baseContent.SalePrice.AsFormattedCurrency(),
                    IsAvailable = isAvaliable,
                    InventoryCount = baseContent.TotalInventoryCount,
                    OutOfStockPurchase = baseContent.OutOfStockPurchase,
                    IsForVariant = isVariant
                };

            return this.OnCreate(row, baseContent);
        }

        /// <summary>
        /// Creates a <see cref="IProductDataTableRow"/> from <see cref="ProductDisplayBase"/>.
        /// </summary>
        /// <param name="baseProduct">
        /// The <see cref="ProductDisplayBase"/>.
        /// </param>
        /// <param name="isVariant">
        /// A value indicating whether or not the row represents a variant.
        /// </param>
        /// <returns>
        /// The <see cref="TRow"/>.
        /// </returns>
        public TRow Create(ProductDisplayBase baseProduct, bool isVariant = true)
        {
            var row = new TRow { IsForVariant = isVariant };

            return this.OnCreate(row, baseProduct);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TTable"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="table">
        /// The <see cref="TTable"/>.
        /// </param>
        /// <param name="productContent">
        /// The product content.
        /// </param>
        /// <returns>
        /// The modified <see cref="TTable"/>.
        /// </returns>
        public TTable OnCreate(TTable table, IProductContent productContent)
        {
            return table;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TTable"/> from <see cref="ProductDisplay"/>.
        /// </summary>
        /// <param name="table">
        /// The <see cref="TTable"/>.
        /// </param>
        /// <param name="productDisplay">
        /// The <see cref="ProductDisplay"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="TTable"/>.
        /// </returns>
        public TTable OnCreate(TTable table, ProductDisplay productDisplay)
        {
            return table;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TRow"/> from <see cref="IProductContentBase"/>.
        /// </summary>
        /// <param name="row">
        /// The <see cref="TRow"/>.
        /// </param>
        /// <param name="baseContent">
        /// The <see cref="IProductContentBase"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="TRow"/>.
        /// </returns>
        public TRow OnCreate(TRow row, IProductContentBase baseContent)
        {
            return row;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TRow"/> from <see cref="ProductDisplayBase"/>.
        /// </summary>
        /// <param name="row">
        /// The <see cref="TRow"/>.
        /// </param>
        /// <param name="baseDisplay">
        /// The <see cref="ProductDisplayBase"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="TRow"/>.
        /// </returns>
        public TRow OnCreate(TRow row, ProductDisplayBase baseDisplay)
        {
            return row;
        }
    }
}