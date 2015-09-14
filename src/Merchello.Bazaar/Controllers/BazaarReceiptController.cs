namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Bazaar.Attributes;

    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar receipt controller.
    /// </summary>
    [PluginController("Bazaar")]
    [RequireSsl("Bazaar:RequireSsl")]
    public class BazaarReceiptController : BazaarControllerBase
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
            var key = CustomerContext.GetValue("invoiceKey");
            if (string.IsNullOrEmpty(key))
            {
                var nullReference = new NullReferenceException("Invoice key was not found in the customer context");
                LogHelper.Error<BazaarReceiptController>("Could not find a reference to the invoice key", nullReference);
                var store = model.Content.Ancestor("BazaarStore");
                return Redirect(store.Url);
            }

            var invoiceKey = new Guid(key);

            var invoice = MerchelloServices.InvoiceService.GetByKey(invoiceKey);
           
            var viewModel = ViewModelFactory.CreateReceipt(model, invoice);

            return this.View(viewModel.ThemeViewPath("Receipt"), viewModel);
        }
    }
}