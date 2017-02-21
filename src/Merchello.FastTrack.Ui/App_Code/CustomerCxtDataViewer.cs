namespace Merchello.FastTrack.Tests
{
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Pluggable;

    using Umbraco.Web;

    public class CustomerCxtDataViewer
    {
        private readonly CustomerContextBase _ctx;

        public CustomerCxtDataViewer()
        {
            _ctx = PluggableObjectHelper.GetInstance<CustomerContextBase>("CustomerContext", UmbracoContext.Current);
        }

        public CustomerContextBase CustomerContext
        {
            get
            {
                return _ctx;
            }
        }

        public ICustomerBase CurrentCustomer
        {
            get
            {
                return _ctx.CurrentCustomer;
            }
        }
    }
}