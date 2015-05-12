namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core.Gateways;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// Payment method controller for the Cash Payment Method.
    /// </summary>
    [PluginController("Bazaar")]
    [GatewayMethodUi("CashPaymentMethod")]
    public class CashPaymentMethodController : BazaarPaymentMethodFormControllerBase
    {
        /// <summary>
        /// Responsible for rendering the Cash Payment Method Form.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult RenderForm(CheckoutConfirmationForm model)
        {
            return this.PartialView(PathHelper.GetThemePartialViewPath(model.ThemeName, "CashPaymentMethodForm"), model);
        }
    }
}