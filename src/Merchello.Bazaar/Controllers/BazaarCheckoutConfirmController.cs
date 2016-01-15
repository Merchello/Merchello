namespace Merchello.Bazaar.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Attributes;
    using Merchello.Bazaar.Models;
    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Ui;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar checkout confirm controller.
    /// </summary>
    [PluginController("Bazaar")]
    [RequireSsl("Bazaar:RequireSsl")]
    public partial class BazaarCheckoutConfirmController : CheckoutControllerBase
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

            // Get the checkout manager
            var checkoutManager = Basket.GetCheckoutManager();

            var shipmentRateQuotes = Enumerable.Empty<IShipmentRateQuote>().ToArray();
            
            // The default basket packaging strategy only creates a single shipment
            var shipment = Basket.PackageBasket(checkoutManager.Customer.GetShipToAddress()).FirstOrDefault();
            
            if (shipment != null)
            {
                var invoice = checkoutManager.Payment.PrepareInvoice();

                // Quote the shipment
                shipmentRateQuotes = shipment.ShipmentRateQuotes().ToArray();
                if (shipmentRateQuotes.Any() && !invoice.ShippingLineItems().Any())
                {
                    //// Assume the first selection.  Customer will be able to update this later
                    //// but this allows for a taxation calculation as well in the event shipping charges
                    //// are taxable.
                    checkoutManager.Shipping.SaveShipmentRateQuote(shipmentRateQuotes.First());
                }
            }            

            var paymentMethods = GatewayContext.Payment.GetPaymentGatewayMethods().ToArray();

            var paymentMethodInfos = new List<PaymentMethodUiInfo>();
            foreach (var method in paymentMethods)
            {
                var att = method.GetType().GetCustomAttribute<GatewayMethodUiAttribute>(false);

                var alias = att == null ? string.Empty : att.Alias;
               
                paymentMethodInfos.Add(new PaymentMethodUiInfo()
                    {
                        Alias = alias.Replace(".", string.Empty),
                        PaymentMethodKey = method.PaymentMethod.Key,
                        UrlActionParams = PaymentMethodUiControllerResolver.Current.GetUrlActionParamsByGatewayMethodUiAlias(alias)
                    });
            }

            var viewModel = ViewModelFactory.CreateCheckoutConfirmation(model, Basket, shipmentRateQuotes, paymentMethods, paymentMethodInfos);

            return this.View(viewModel.ThemeViewPath("CheckoutConfirmation"), viewModel);
        }
    }
}