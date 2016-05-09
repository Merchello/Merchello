namespace Merchello.QuickMart.Controllers
{
    using Merchello.QuickMart.Factories;
    using Merchello.QuickMart.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering and processing addresses in the default checkout process.
    /// </summary>
    [PluginController("QuickMart")]
    public class QuickMartCheckoutAddressController : CheckoutAddressControllerBase<CheckoutBillingAddressModel, CheckoutAddressModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickMartCheckoutAddressController"/> class.
        /// </summary>
        public QuickMartCheckoutAddressController()
            : base(
                  new QuickMartBillingAddressModelFactory(),
                  new CheckoutAddressModelFactory<CheckoutAddressModel>())
        {
        }
    }
}