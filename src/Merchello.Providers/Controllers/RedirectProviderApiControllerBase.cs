namespace Merchello.Providers.Controllers
{
    using System;
    using System.Net.Http;

    using Merchello.Core;
    using Merchello.Web.Reporting;

    /// <summary>
    /// A base class for handling redirecting Payment Provider returns.
    /// </summary>
    public abstract class RedirectProviderApiControllerBase : ReportController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectProviderApiControllerBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        protected RedirectProviderApiControllerBase(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
        }

        public abstract HttpResponseMessage HandleSuccess(Guid invoiceKey, Guid paymentKey, params string[] providerParams);
    }
}