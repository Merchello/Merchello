namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Sales;
    using Merchello.Web;

    using umbraco.BusinessLogic.Actions;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A <see cref="SurfaceController"/> responsible for checkout operations.
    /// </summary>
    [PluginController("Bazaar")]
    public class SalePreparationOperationsController : SurfaceControllerBase
    {
        /// <summary>
        /// Saves addresses during the checkout process.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult SaveAddresses(CheckoutAddressForm model)
        {
            if (!ModelState.IsValid) return this.CurrentUmbracoPage();
            
            return RedirectToUmbracoPage(model.ConfirmSalePageId);
        }
    }
}