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

    public class ProductDataService : IProductDataService
    {
        private readonly IMerchelloContext _merchelloContext;

        [SecuritySafeCritical]
        public ProductDataService()
            : this(MerchelloContext.Current)
        { }

        public ProductDataService(IMerchelloContext merchelloContext)
        {
            //TODO Figure out how to get MerchelloContext instantiated before ExamineManager 
            // attempts to interact with the index
            //Mandate.ParameterNotNull(merchelloContext, "MerchelloContext");
            _merchelloContext = merchelloContext;
        }


        public IEnumerable<IProduct> GetAll()
        {
            return new ProductService().GetPage(1, 100).Items;
        }

        /// <summary>
        /// Returns a list of all property names in the Merchello set being indexed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetIndexFieldNames()
        {
            return ProductIndexer.IndexFieldPolicies.Select(x => x.Name);
        }

    }
}