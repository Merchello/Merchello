namespace Merchello.Web.Store.Controllers
{
    using Merchello.QuickMart.Factories;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Store.Factories;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering and processing addresses in the default checkout process.
    /// </summary>
    [PluginController("Merchello")]
    public class CheckoutAddressController : CheckoutAddressControllerBase<CheckoutAddressModel, CheckoutAddressModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutAddressController"/> class.
        /// </summary>
        public CheckoutAddressController()
            : base(
                  new CheckoutAddressModelFactory<CheckoutAddressModel>(),
                  new CheckoutAddressModelFactory<CheckoutAddressModel>())
        {
        }
    }
}