using System.Security;
using Merchello.Core;

namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using Providers;

    /// <summary>
    /// The customer data service.
    /// </summary>
    public class CustomerDataService : ICustomerDataService
    {
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerDataService"/> class.
        /// </summary>
        [SecuritySafeCritical]
        public CustomerDataService()
            : this(MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerDataService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public CustomerDataService(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>
        ///  The collection of all <see cref="ICustomer"/>.
        /// </returns>
        public IEnumerable<ICustomer> GetAll()
        {
            return new CustomerService().GetAll();
        }

        /// <summary>
        /// Returns a list of all property names in the Merchello set being indexed
        /// </summary>
        /// <returns>
        /// The collection of all index field names.
        /// </returns>
        public IEnumerable<string> GetIndexFieldNames()
        {
            return CustomerIndexer.IndexFieldPolicies.Select(x => x.Name);
        }
    }
}