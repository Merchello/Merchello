namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using Core;
    using Core.Models;
    using Core.Services;
    using Providers;
    using Umbraco.Core.Persistence.UnitOfWork;

    /// <summary>
    /// The product data service.
    /// </summary>
    internal class ProductDataService : IProductDataService
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
            ////TODO Figure out how to get MerchelloContext instantiated before ExamineManager 
            //// attempts to interact with the index
            ////Mandate.ParameterNotNull(merchelloContext, "MerchelloContext");
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
            return new ProductService().GetPage(1, 100).Items;
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