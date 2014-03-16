using System.Collections.Generic;
using System.Linq;
using System.Security;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine.Providers;

namespace Merchello.Examine.DataServices
{
    public class OrderDataService : IOrderDataService
    {
        private readonly IMerchelloContext _merchelloContext;

        [SecuritySafeCritical]
        public OrderDataService()
            : this(MerchelloContext.Current)
        { }

        public OrderDataService(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }


        public IEnumerable<IOrder> GetAll()
        {
            return new OrderService().GetAll();
        }



        public IEnumerable<string> GetIndexFieldNames()
        {
            return InvoiceIndexer.IndexFieldPolicies.Select(x => x.Name);
        } 
    }
}