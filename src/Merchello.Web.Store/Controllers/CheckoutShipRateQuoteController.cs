namespace Merchello.Web.Store.Controllers
{
    using Merchello.Core;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible quoting shipment rate quotes.
    /// </summary>
    [PluginController("Merchello")]
    public class CheckoutShipRateQuoteController : CheckoutShipRateQuoteControllerBase<CheckoutShipRateQuoteModel>
    {
    }
}