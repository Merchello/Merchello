namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// Controller to render the first page of the checkout.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarCheckoutController : RenderControllerBase
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
            var viewModel = this.Populate(new CheckoutModel(model.Content));

            return this.View(viewModel.ThemeViewPath("Checkout"), viewModel);
        }
    }
}