namespace Merchello.Implementation.Controllers
{
    using Merchello.Implementation.Controllers.Base;
    using Merchello.Implementation.Factories;
    using Merchello.Implementation.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering and processing addresses in the default checkout process.
    /// </summary>
    [PluginController("Merchello")]
    public class DefaultCheckoutAddressController : CheckoutAddressControllerBase<CheckoutBillingAddressModel, CheckoutAddressModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCheckoutAddressController"/> class.
        /// </summary>
        public DefaultCheckoutAddressController()
            : base(
                  new CheckoutBillingAddressModelFactory(),
                  new CheckoutAddressModelFactory<CheckoutAddressModel>())
        {
        }
    }
}