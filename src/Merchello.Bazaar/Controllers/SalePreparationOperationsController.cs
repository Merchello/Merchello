namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
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
        /// The save billing address.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult SaveBillingAddress(AddressFormModel model)
        {
            if (!ModelState.IsValid) return this.CurrentUmbracoPage();

            var preparation = Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;
            preparation.SaveBillToAddress(model);

            if (model.BillingIsShipping) preparation.SaveShipToAddress(model);

            return RedirectToUmbracoPage(model.ContinuePageId);
        }
    }
}