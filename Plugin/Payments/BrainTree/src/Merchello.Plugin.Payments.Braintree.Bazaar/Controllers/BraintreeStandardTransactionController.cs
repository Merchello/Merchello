namespace Merchello.Plugin.Payments.Braintree.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Core.Gateways;

    /// <summary>
    /// A controller to Authorize/Capture Braintree payments.
    /// </summary>
    [GatewayMethodUi("BrainTree.StandardTransaction")]
    public class BraintreeStandardTransactionController : BraintreeTransactionControllerBase
    {
        /// <summary>
        /// Responsible for rendering the payment form for capturing standard payments via Braintree.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/> Partial View.
        /// </returns>
        [ChildActionOnly]
        public override ActionResult RenderForm()
        {
            return this.PartialView(BraintreePartial("BraintreeStandardTransaction"));
        }
    }
}