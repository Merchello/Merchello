﻿namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using Core;
    using Core.Models;
    using Core.Services;
    using Providers;

    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.UnitOfWork;

    /// <summary>
    /// The product data service.
    /// </summary>
    internal class ProductDataService : DataServiceBase, IProductDataService
    {
        /// <summary>
        /// The _merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDataService"/> class.
        /// </summary>
        [SecuritySafeCritical]
        public ProductDataService()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDataService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ProductDataService(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
        public IEnumerable<IProduct> GetAll()
        {
            return MerchelloContext.HasCurrent
                       ? MerchelloContext.Current.Services.ProductService.GetAll()
                       : new ProductService().GetAll();
        }

        /// <summary>
        /// Returns a list of all property names in the Merchello set being indexed
        /// </summary>
        /// <returns>The <see cref="IEnumerable{String}"/></returns>
        public IEnumerable<string> GetIndexFieldNames()
        {
            return ProductIndexer.IndexFieldPolicies.Select(x => x.Name);
        }

    }
}