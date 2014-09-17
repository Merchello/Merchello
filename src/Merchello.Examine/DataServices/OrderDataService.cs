namespace Merchello.Examine.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Examine.Providers;

    /// <summary>
    /// The order data service.
    /// </summary>
    internal class OrderDataService : IOrderDataService
    {
        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDataService"/> class.
        /// </summary>
        [SecuritySafeCritical]
        public OrderDataService()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDataService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public OrderDataService(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IOrder}"/>.
        /// </returns>
        public IEnumerable<IOrder> GetAll()
        {
            return new OrderService().GetPage(1, 100).Items;
        }

        /// <summary>
        /// The get index field names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public IEnumerable<string> GetIndexFieldNames()
        {
            return OrderIndexer.IndexFieldPolicies.Select(x => x.Name);
        } 
    }
}