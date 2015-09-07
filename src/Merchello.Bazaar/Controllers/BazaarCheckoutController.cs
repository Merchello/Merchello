namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;
    using System.Linq;

    using Merchello.Bazaar.Attributes;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Controller to render the first page of the checkout.
    /// </summary>
    [PluginController("Bazaar")]
    [RequireSsl("Bazaar:RequireSsl")]
    public class BazaarCheckoutController : CheckoutControllerBase
    {
        /// <summary>
        /// The index <see cref="ActionResult"/>.
        /// </summary>
        /// <param name="model">
        /// The current render model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Index(RenderModel model)
        {
            // Don't display the checkout form if there are no products (either due to timeout or manual URL entered)
            if (!Basket.Items.Any())
            {
                var basketModel = ViewModelFactory.CreateBasket(model, Basket);
                return View(basketModel.ThemeViewPath("Basket"), basketModel);
            }

            var viewModel = ViewModelFactory.CreateCheckout(model, Basket, AllCountries, AllowedShipCountries);

            return this.View(viewModel.ThemeViewPath("Checkout"), viewModel);
        }
    }
}