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

    using Umbraco.Core;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar checkout confirm controller.
    /// </summary>
    [PluginController("Bazaar")]
    [RequireSsl("Bazaar:RequireSsl")]
    public class BazaarCheckoutConfirmController : CheckoutControllerBase
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

            // get the basket sale preparation
            var preparation = Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;
            
            var shipmentRateQuotes = Enumerable.Empty<IShipmentRateQuote>().ToArray();
            
            // The default basket packaging strategy only creates a single shipment
            var shipment = Basket.PackageBasket(preparation.GetShipToAddress()).FirstOrDefault();
            
            if (shipment != null)
            {
                var invoice = preparation.PrepareInvoice();

                // Quote the shipment
                shipmentRateQuotes = shipment.ShipmentRateQuotes().ToArray();
                if (shipmentRateQuotes.Any() && !invoice.ShippingLineItems().Any())
                {
                    //// Assume the first selection.  Customer will be able to update this later
                    //// but this allows for a taxation calculation as well in the event shipping charges
                    //// are taxable.
                    preparation.SaveShipmentRateQuote(shipmentRateQuotes.First());
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