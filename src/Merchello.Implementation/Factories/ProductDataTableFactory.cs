namespace Merchello.Implementation.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Implementation.Models;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// The product data table factory.
    /// </summary>
    /// <typeparam name="TTable">
    /// The type of of <see cref="IProductDataTable"/>
    /// </typeparam>
    /// <typeparam name="TTableRow">
    /// The type of <see cref="IProductDataTableRow"/>
    /// </typeparam>
    public class ProductDataTableFactory<TTable, TTableRow>
        where TTable : class, IProductDataTable, new()
        where TTableRow : class, IProductDataTableRow, new()
    {
        /// <summary>
        /// Creates a <see cref="IProductDataTable"/> from <see cref="IProductContent"/>.
        /// </summary>
        /// <param name="productContent">
        /// The <see cref="IProductContent"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TTable"/>.
        /// </returns>
        public TTable Create(IProductContent productContent)
        {
            var table = new TTable();

            var rows = new List<TTableRow>();
            rows.AddRange(productContent.ProductVariants.Select(variant => this.Create(variant)));

            return OnCreate(table, productContent);
        }

        /// <summary>
        /// Creates a <see cref="IProductDataTable"/> from <see cref="ProductDisplay"/>.
        /// </summary>
        /// <param name="productDisplay">
        /// The <see cref="ProductDisplay"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TTable"/>.
        /// </returns>
        public TTable Create(ProductDisplay productDisplay)
        {
            var table = new TTable();
       
            var rows = new List<TTableRow> { this.Create(productDisplay, false) };
            rows.AddRange(productDisplay.ProductVariants.Select(variant => this.Create(variant)));

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
        /// The <see cref="TTableRow"/>.
        /// </returns>
        public TTableRow Create(IProductContentBase baseContent, bool isVariant = true)
        {
            var row = new TTableRow { IsForVariant = isVariant };

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
        /// The <see cref="TTableRow"/>.
        /// </returns>
        public TTableRow Create(ProductDisplayBase baseProduct, bool isVariant = true)
        {
            var row = new TTableRow { IsForVariant = isVariant };

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
        /// Allows for overriding the creation of <see cref="TTableRow"/> from <see cref="IProductContentBase"/>.
        /// </summary>
        /// <param name="row">
        /// The <see cref="TTableRow"/>.
        /// </param>
        /// <param name="baseContent">
        /// The <see cref="IProductContentBase"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="TTableRow"/>.
        /// </returns>
        public TTableRow OnCreate(TTableRow row, IProductContentBase baseContent)
        {
            return row;
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="TTableRow"/> from <see cref="ProductDisplayBase"/>.
        /// </summary>
        /// <param name="row">
        /// The <see cref="TTableRow"/>.
        /// </param>
        /// <param name="baseDisplay">
        /// The <see cref="ProductDisplayBase"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="TTableRow"/>.
        /// </returns>
        public TTableRow OnCreate(TTableRow row, ProductDisplayBase baseDisplay)
        {
            return row;
        }
    }
}