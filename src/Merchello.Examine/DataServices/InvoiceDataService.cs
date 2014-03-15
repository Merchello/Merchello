using System.Collections.Generic;
using System.Linq;
using System.Security;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Examine.Providers;

namespace Merchello.Examine.DataServices
{
    public class InvoiceDataService : IInvoiceDataService
    {
        private readonly IMerchelloContext _merchelloContext;

        [SecuritySafeCritical]
        public InvoiceDataService()
            : this(MerchelloContext.Current)
        { }

        public InvoiceDataService(IMerchelloContext merchelloContext)
        {
            _merchelloContext = merchelloContext;
        }


        public IEnumerable<IInvoice> GetAll()
        {
            return new InvoiceService().GetAll();
        }



        public IEnumerable<string> GetIndexFieldNames()
        {
            return InvoiceIndexer.IndexFieldPolicies.Select(x => x.Name);
        }
    }
}