namespace Merchello.Implementation.Controllers
{
    using System.Web.Mvc;

    using Merchello.Implementation.Controllers.Base;
    using Merchello.Implementation.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("Merchello")]
    public class DefaultCheckoutSummaryController : CheckoutSummaryControllerBase<CheckoutSummaryModel>
    {
        /// <summary>
        /// Renders the Basket Summary.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult BasketSummary()
        {
            var model = new CheckoutSummaryModel();
            return this.PartialView(model);
        }
    }
}