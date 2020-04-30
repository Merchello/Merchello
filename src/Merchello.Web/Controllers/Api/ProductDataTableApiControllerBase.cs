namespace Merchello.Web.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Web;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// An API controller for handling product data tables.
    /// </summary>
    /// <typeparam name="TTable">
    /// The type of <see cref="IProductDataTable{TRow}"/>
    /// </typeparam>
    /// <typeparam name="TRow">
    /// The type of <see cref="IProductDataTableRow"/>
    /// </typeparam>
    [Merchello.Web.WebApi.JsonCamelCaseFormatter]
    public abstract class ProductDataTableApiControllerBase<TTable, TRow> : UmbracoApiController
        where TTable : class, IProductDataTable<TRow>, new()
        where TRow : class, IProductDataTableRow, new()
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchelloHelper;

        /// <summary>
        /// A factory responsible for building the <see cref="TTable"/>.
        /// </summary>
        private readonly ProductDataTableFactory<TTable, TRow> _productDataTableFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDataTableApiControllerBase{TTable,TRow}"/> class.
        /// </summary>
        protected ProductDataTableApiControllerBase()
            : this(
                  new MerchelloHelper(),
                  new ProductDataTableFactory<TTable, TRow>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDataTableApiControllerBase{TTable,TRow}"/> class.
        /// </summary>
        /// <param name="merchelloHelper">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        /// <param name="productDataTableFactory">
        /// The <see cref="ProductDataTableFactory{TTable, TTableRow}"/>.
        /// </param>
        protected ProductDataTableApiControllerBase(
            MerchelloHelper merchelloHelper,
            ProductDataTableFactory<TTable, TRow> productDataTableFactory)
        {
            Ensure.ParameterNotNull(merchelloHelper, "merchell");
            Ensure.ParameterNotNull(productDataTableFactory, "productDataTableFactory");

            this._merchelloHelper = merchelloHelper;
            this._productDataTableFactory = productDataTableFactory;
        }

        /// <summary>
        /// Gets a collection of <see cref="TTable"/>.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TTable}"/>.
        /// </returns>
        [HttpPost]
        public virtual IEnumerable<TTable> PostGetProductDataTables(Guid[] keys)
        {
            if (!keys.Any()) return Enumerable.Empty<TTable>();

            return
                keys.Select(this._merchelloHelper.TypedProductContent)
                    .Where(x => x != null)
                    .Select(product => this._productDataTableFactory.Create(product))
                    .ToArray();
        }
    }
}