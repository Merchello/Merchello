namespace Merchello.Implementation.Controllers.Base.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;
    using Merchello.Web;

    using Umbraco.Core;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// An API controller for handling product data tables.
    /// </summary>
    /// <typeparam name="TTable">
    /// The type of <see cref="IProductDataTable"/>
    /// </typeparam>
    /// <typeparam name="TRow">
    /// The type of <see cref="IProductDataTableRow"/>
    /// </typeparam>
    public abstract class ProductDataTableApiControllerBase<TTable, TRow> : UmbracoApiController
        where TTable : class, IProductDataTable, new()
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
            Mandate.ParameterNotNull(merchelloHelper, "merchell");
            Mandate.ParameterNotNull(productDataTableFactory, "productDataTableFactory");

            this._merchelloHelper = merchelloHelper;
            _productDataTableFactory = productDataTableFactory;
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
        public virtual IEnumerable<TTable> GetProductDataTables(Guid[] keys)
        {
            if (!keys.Any()) return Enumerable.Empty<TTable>();

            return
                keys.Select(_merchelloHelper.TypedProductContent)
                    .Where(x => x != null)
                    .Select(product => _productDataTableFactory.Create(product))
                    .ToArray();
        }
    }
}