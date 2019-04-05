namespace Merchello.Providers.Payment.Braintree.Controllers.Payment
{
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible for rendering and processing Braintree PayPal payments.
    /// </summary>
    [PluginController("FastTrack")]
    [GatewayMethodUi("BrainTree.PayPal.OneTime")]
    public class BraintreePayPalController : FastTrackBraintreeControllerBase<BraintreePaymentModel>
    {
       
    }
}